using System;
using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper.Abstractions;

namespace RabbitMQ.Client.Wrapper
{
	public class RabbitMQConsumer
	{
		private readonly ILogger<RabbitMQConsumer> _logger = null;
		private readonly IRabbitMQConfig _rabbitMQConfig = null;
		private readonly IRabbitMQClientWrapper _rabbitMQClientWrapper = null;
		public static string LastReceivedMessage { get; private set; }
		public static object _lock = new object();

		public RabbitMQConsumer(ILogger<RabbitMQConsumer> logger, IRabbitMQConfig rabbitMQConfig, IRabbitMQClientWrapper rabbitMQClientWrapper)
		{
			_logger = logger;
			_rabbitMQConfig = rabbitMQConfig;
			_rabbitMQClientWrapper = rabbitMQClientWrapper;
		}

		public void Register()
		{
			_rabbitMQClientWrapper.Initialize();
			_rabbitMQClientWrapper.EventingBasicConsumer.Received += (model, ea) =>
			{
				var body = ea.Body;
				var message = Encoding.UTF8.GetString(body);
				var routingKey = ea.RoutingKey;
				var message80 = message.Substring(0, Math.Min(80, message.Length));

				_logger.LogInformation($" {nameof(RabbitMQConsumer)}> [x] Received '{routingKey}':'{message80}'(80 char only)");

				lock (_lock)
				{
					LastReceivedMessage = message;
				}
			};

			_rabbitMQClientWrapper.BasicConsume();
			_logger.LogInformation($" {nameof(RabbitMQConsumer)} > Waiting for messages from queue='{_rabbitMQConfig.Queue.Name}'");
		}
	}
}
