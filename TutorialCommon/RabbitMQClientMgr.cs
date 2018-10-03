﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace TutorialCommon
{
	public class RabbitMQClientMgr
	{
		#region Private

		private readonly ILogger _logger = null;
		private readonly IRabbitMQConfig _rabbitMQConfig = null;
		private EventingBasicConsumer _eventingBasicConsumer = null;

		#endregion

		#region Public

		public RabbitMQClientMgr(ILogger<RabbitMQClientMgr> logger, IRabbitMQConfig rabbitMQConfig)
		{
			_logger = logger;
			_rabbitMQConfig = rabbitMQConfig;

			_logger.LogInformation($"{nameof(RabbitMQClientMgr)} constructed");
		}

		public IConnection Connection { get; private set; }

		public IModel Model { get; private set; }

		public void Initialize()
		{
			var connectionFactory = new ConnectionFactory();
			connectionFactory.HostName = _rabbitMQConfig.Connection.HostName;
			connectionFactory.UserName = _rabbitMQConfig.Connection.UserName;
			connectionFactory.Password = _rabbitMQConfig.Connection.Password;

			Connection = connectionFactory.CreateConnection();
			_logger.LogInformation($"Connection created {Connection}");
			Model = Connection.CreateModel();
			_logger.LogInformation($"Model created {Model}");

			IExchangeConfig exchangeConfig = _rabbitMQConfig.Exchange;
			_logger.LogInformation($"exchangeConfig: {JsonConvert.SerializeObject(exchangeConfig)}");

			if (exchangeConfig != null)
			{
				Model.ExchangeDeclare(
						exchange: exchangeConfig.Name,
						type: exchangeConfig.Type,
						autoDelete: exchangeConfig.AutoDelete.Value,
						durable: exchangeConfig.Durable.Value,
						arguments: exchangeConfig.Arguments);
				_logger.LogInformation($"ExchangeDeclare completed.  exchange={exchangeConfig.Name}, type={exchangeConfig.Type}, autoDelete={exchangeConfig.AutoDelete}, durable={exchangeConfig.Durable}, arguments={exchangeConfig.Arguments}");
			}

			if (_rabbitMQConfig.Queue != null)
			{
				IQueueConfig queueConfig = _rabbitMQConfig.Queue;
				_logger.LogInformation($"queueConfig: {JsonConvert.SerializeObject(queueConfig)}");

				var queueDeclareOk = Model.QueueDeclare(
					queue: queueConfig.Name,
					exclusive: queueConfig.Exclusive.Value,
					autoDelete: queueConfig.AutoDelete.Value,
					durable: queueConfig.Durable.Value,
					arguments: queueConfig.Arguments);
				_logger.LogInformation($"QueueDeclare completed.  queue={queueConfig.Name}, exclusive={queueConfig.Exclusive}, autoDelete={queueConfig.AutoDelete}, durable={queueConfig.Durable}, arguments={queueConfig.Arguments}, queueDeclareOk ={queueDeclareOk}");

				if (queueConfig.Name.Length == 0)
				{
					queueConfig.SetAutoGeneratedQueueName(queueDeclareOk.QueueName);
				}
			}

			if (_rabbitMQConfig.BasicQosConfig != null)
			{
				Model.BasicQos(
					prefetchSize: _rabbitMQConfig.BasicQosConfig.PrefetchSize.Value,
					prefetchCount: _rabbitMQConfig.BasicQosConfig.PrefetchCount.Value,
					global: _rabbitMQConfig.BasicQosConfig.Global.Value);
			}

			if (_rabbitMQConfig.QueueBinding != null)
			{
				_logger.LogInformation($"queueBindingConfig: {JsonConvert.SerializeObject(_rabbitMQConfig.QueueBinding)}");

				foreach (string routingKey in _rabbitMQConfig.QueueBinding.RoutingKeys)
				{
					QueueBind(routingKey: routingKey);
				}
			}
		}

		public void BasicPublish(byte[] body)
		{
			BasicPublish(_rabbitMQConfig.BasicPublishConfig.RoutingKey, body);
		}

		public void BasicPublish(string routingKey, byte[] body)
		{
			string exchange = _rabbitMQConfig.Exchange != null ? _rabbitMQConfig.Exchange.Name : "";
			IBasicProperties basicProperties = null;
			if (_rabbitMQConfig.BasicPublishConfig != null && _rabbitMQConfig.BasicPublishConfig.BasicProperitesConfig != null)
			{
				basicProperties = Model.CreateBasicProperties();
				basicProperties.Persistent = _rabbitMQConfig.BasicPublishConfig.BasicProperitesConfig.Persistent.Value;
			}
			Model.BasicPublish(
				exchange: exchange,
				routingKey: routingKey,
				mandatory: false,
				basicProperties: basicProperties,
				body: body);
			_logger.LogInformation($"BasicPublish completed: exchange={exchange}, routingKey={routingKey}, mandatory=false, basicProperties=null,body={body}");
		}

		public void QueueBind(string routingKey)
		{
			Model.QueueBind(
				queue: _rabbitMQConfig.Queue.Name,
				exchange: _rabbitMQConfig.Exchange.Name,
				routingKey: routingKey);
			_logger.LogInformation($"QueueBind completed: Name={_rabbitMQConfig.Queue.Name}, exchange={_rabbitMQConfig.Exchange.Name} routingKey={routingKey}");
		}

		public void BasicConsume()
		{
			bool autoAck = _rabbitMQConfig.BasicConsumeConfig != null ? _rabbitMQConfig.BasicConsumeConfig.AutoAck.Value : true;
			Model.BasicConsume(
				queue: _rabbitMQConfig.Queue.Name,
				autoAck: autoAck,
				consumer: EventingBasicConsumer);
			_logger.LogInformation($"BasicConsume completed: queue={_rabbitMQConfig.Queue.Name}, autoAck={autoAck}, consumer={EventingBasicConsumer}");
		}

		public void BasicAck(ulong deliveryTag, bool multiple)
		{
			Model.BasicAck(deliveryTag: deliveryTag, multiple: multiple);
			_logger.LogInformation($"BasicAck completed: deliveryTag={deliveryTag}, multiple={multiple}");
		}

		public EventingBasicConsumer EventingBasicConsumer
		{
			get
			{
				if (_eventingBasicConsumer == null)
				{
					_eventingBasicConsumer = new EventingBasicConsumer(Model);
				}
				return _eventingBasicConsumer;
			}
		}

		public void Uninitialize()
		{
			if (Model != null)
			{
				Model.Dispose();
				Model = null;
			}

			if (Connection != null)
			{
				Connection.Dispose();
				Connection = null;
			}
		}

		#endregion
	}
}
