using System;
using System.Collections.Generic;
using DotNetCoreCommon;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper.Abstractions;

namespace RabbitMQ.Client.Wrapper
{
	public class RabbitMQConfig : IRabbitMQConfig
	{
		#region Private

		private readonly ILogger _logger = null;
		private readonly IConfiguration _configuration = null;
		private readonly Guid _id = Guid.NewGuid();
		private bool _initialized = false;

		#endregion

		#region Public 
		public virtual ConnectionConfig Connection { get; set; } = null;
		public virtual ExchangeConfig Exchange { get; set; } = null;
		public virtual QueueConfig Queue { get; set; } = null;
		public virtual QueueBindingConfig QueueBinding { get; set; } = null;
		public virtual BasicPublishConfig BasicPublishConfig { get; set; } = null;
		public virtual ConfirmSelectConfig ConfirmSelectConfig { get; set; } = null;
		public virtual BasicQosConfig BasicQosConfig { get; set; } = null;
		public virtual BasicConsumeConfig BasicConsumeConfig { get; set; } = null;
		public virtual TestConfig TestConfig { get; set; } = null;

		public RabbitMQConfig(ILogger<RabbitMQConfig> logger, IConfiguration configuration)
		{
			_logger = logger;
			_configuration = configuration;
			_logger.LogDebug($"Constructed. id={_id}");
		}

		public virtual void Initialize()
		{
			_logger.LogInformation($"Initializing. _initialized={_initialized}");
			if (_initialized) return;

			IConfigurationSection rabbitMQSection = _configuration.GetSection(RabbitMQJsonNames.RabbitMQ.Section);

			foreach (IConfigurationSection childSection in rabbitMQSection.GetChildren())
			{
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.Connection.Section:
						Connection = ConnectionConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.Exchange.Section:
						Exchange = ExchangeConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.Queue.Section:
						Queue = QueueConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.QueueBinding.Section:
						QueueBinding = QueueBindingConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.BasicPublish.Section:
						BasicPublishConfig = BasicPublishConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.ConfirmSelect.Section:
						ConfirmSelectConfig = ConfirmSelectConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.BasicQos.Section:
						BasicQosConfig = BasicQosConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.BasicConsume.Section:
						BasicConsumeConfig = BasicConsumeConfig.Initialize(childSection);
						break;
					case RabbitMQJsonNames.RabbitMQ.Test.Section:
						TestConfig = TestConfig.Initialize(childSection);
						break;
					default:
						break;
				}
			}
			_initialized = true;
		}

		#endregion

		#region IRabbitMQConfig Interface
		IConnectionConfig IRabbitMQConfig.Connection => Connection;
		IExchangeConfig IRabbitMQConfig.Exchange => Exchange;
		IQueueConfig IRabbitMQConfig.Queue => Queue;
		IQueueBindingConfig IRabbitMQConfig.QueueBinding => QueueBinding;
		IBasicPublishConfig IRabbitMQConfig.BasicPublishConfig => BasicPublishConfig;
		IConfirmSelectConfig IRabbitMQConfig.ConfirmSelectConfig => ConfirmSelectConfig;
		IBasicQosConfig IRabbitMQConfig.BasicQosConfig => BasicQosConfig;
		IBasicConsumeConfig IRabbitMQConfig.BasicConsumeConfig => BasicConsumeConfig;
		ITestConfig IRabbitMQConfig.TestConfig => TestConfig;

		#endregion
	}

	public class ConnectionConfig : IConnectionConfig
	{
		#region Public 
		public virtual string HostName { get; set; } = string.Empty;
		public virtual string UserName { get; set; } = string.Empty;
		public virtual string Password { get; set; } = string.Empty;

		public static ConnectionConfig Initialize(IConfigurationSection connectionSection)
		{
			if (connectionSection == null) return null;

			ConnectionConfig connectionConfig = new ConnectionConfig();
			bool hasChildren = false;
			foreach (IConfigurationSection childSection in connectionSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.Connection.HostName:
						connectionConfig.HostName = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.Connection.UserName:
						connectionConfig.UserName = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.Connection.Password:
						connectionConfig.Password = childSection.Value;
						break;
					default:
						break;
				}
			}
			if (hasChildren) return connectionConfig;
			return null;
		}
		#endregion

		#region IConnectionConfig interface

		string IConnectionConfig.HostName => HostName;
		string IConnectionConfig.UserName => UserName;
		string IConnectionConfig.Password => Password;

		#endregion

	}

	public class ExchangeConfig : IExchangeConfig
	{
		#region Public

		public virtual string Name { get; set; } = string.Empty;

		public virtual string Type { get; set; } = string.Empty;

		public virtual bool Durable { get; set; } = false;

		public virtual bool AutoDelete { get; set; } = false;

		public virtual Dictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

		public static ExchangeConfig Initialize(IConfigurationSection exchangeSection)
		{
			if (exchangeSection == null) return null;

			ExchangeConfig exchangeConfig = new ExchangeConfig();
			bool hasChildren = false;
			foreach (IConfigurationSection childSection in exchangeSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.Exchange.Name:
						exchangeConfig.Name = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.Exchange.Type:
						exchangeConfig.Type = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.Exchange.Durable:
						exchangeConfig.Durable = childSection.Value.ToBoolean();
						break;
					case RabbitMQJsonNames.RabbitMQ.Exchange.AutoDelete:
						exchangeConfig.AutoDelete = childSection.Value.ToBoolean();
						break;
					case RabbitMQJsonNames.RabbitMQ.Exchange.Arguments:
						break;
					default:
						break;
				}
			}
			if (hasChildren) return exchangeConfig;
			return null;
		}
		#endregion

		#region IExchangeConfig interface
		string IExchangeConfig.Name => Name;

		string IExchangeConfig.Type => Type;

		bool? IExchangeConfig.Durable => Durable;

		bool? IExchangeConfig.AutoDelete => AutoDelete;

		IDictionary<string, object> IExchangeConfig.Arguments => Arguments;
		#endregion
	}

	public class QueueConfig : IQueueConfig
	{
		#region Public
		public virtual string Name { get; set; } = string.Empty;

		public virtual void SetAutoGeneratedQueueName(string queueName)
		{
			Name = queueName;
		}

		public virtual bool Exclusive { get; set; } = true;

		public virtual bool Durable { get; set; } = false;

		public virtual bool AutoDelete { get; set; } = true;

		public virtual IDictionary<string, object> Arguments { get; set; } = new Dictionary<string, object>();

		public static QueueConfig Initialize(IConfigurationSection queueSection)
		{
			if (queueSection == null) return null;

			QueueConfig queueConfig = new QueueConfig();
			bool hasChildren = false;
			foreach (IConfigurationSection childSection in queueSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.Queue.Name:
						queueConfig.Name = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.Queue.Exclusive:
						queueConfig.Exclusive = childSection.Value.ToBoolean();
						break;
					case RabbitMQJsonNames.RabbitMQ.Queue.Durable:
						queueConfig.Durable = childSection.Value.ToBoolean(); ;
						break;
					case RabbitMQJsonNames.RabbitMQ.Queue.AutoDelete:
						queueConfig.AutoDelete = childSection.Value.ToBoolean();
						break;
					case RabbitMQJsonNames.RabbitMQ.Queue.Arguments:
						break;
					default:
						break;
				}
			}
			if (hasChildren) return queueConfig;
			return null;
		}
		#endregion

		#region IQueueConfig interface
		string IQueueConfig.Name => Name;

		bool? IQueueConfig.Exclusive => Exclusive;

		bool? IQueueConfig.Durable => Durable;

		bool? IQueueConfig.AutoDelete => AutoDelete;

		IDictionary<string, object> IQueueConfig.Arguments => Arguments;

		void IQueueConfig.SetAutoGeneratedQueueName(string queueName)
		{
			SetAutoGeneratedQueueName(queueName);
		}
		#endregion
	}

	public class QueueBindingConfig : IQueueBindingConfig
	{
		#region Public
		public virtual List<string> RoutingKeys { get; set; } = new List<string>();

		public static QueueBindingConfig Initialize(IConfigurationSection queueBindingSection)
		{
			if (queueBindingSection == null) return null;

			bool hasChildren = false;
			QueueBindingConfig queueBindingConfig = new QueueBindingConfig();
			foreach (IConfigurationSection childSection in queueBindingSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.QueueBinding.RoutingKeys:
						foreach (IConfigurationSection routingKeySection in childSection.GetChildren())
						{
							queueBindingConfig.RoutingKeys.Add(routingKeySection.Value);
						}
						break;
					default:
						break;
				}
			}
			if (hasChildren) return queueBindingConfig;
			return null;
		}
		#endregion

		#region IQueueBindingConfig interface
		IList<string> IQueueBindingConfig.RoutingKeys => RoutingKeys;

		#endregion
	}

	public class BasicPublishConfig : IBasicPublishConfig
	{
		#region Public
		public virtual BasicProperitesConfig BasicProperitesConfig { get; set; }
		public virtual string RoutineKey { get; set; }

		public static BasicPublishConfig Initialize(IConfigurationSection basicPublishSection)
		{
			if (basicPublishSection == null) return null;

			bool hasChildren = false;
			BasicPublishConfig basicPublishConfig = new BasicPublishConfig();
			foreach (IConfigurationSection childSection in basicPublishSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.BasicPublish.RoutingKey:
						basicPublishConfig.RoutineKey = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.BasicPublish.BasicProperites.Section:
						basicPublishConfig.BasicProperitesConfig = BasicProperitesConfig.Initialize(childSection);
						break;
					default:
						break;
				}
			}
			if (hasChildren) return basicPublishConfig;
			return null;
		}
		#endregion

		#region IQueueBindingConfig interface

		IBasicProperitesConfig IBasicPublishConfig.BasicProperitesConfig => BasicProperitesConfig;
		string IBasicPublishConfig.RoutingKey => RoutineKey;

		#endregion
	}

	public class BasicProperitesConfig : IBasicProperitesConfig
	{
		#region Public
		public virtual bool Persistent { get; set; } = false;

		public static BasicProperitesConfig Initialize(IConfigurationSection basicPropertiesSection)
		{
			if (basicPropertiesSection == null) return null;

			bool hasChildren = false;

			BasicProperitesConfig basicProperitesConfig = new BasicProperitesConfig();
			foreach (IConfigurationSection childSection in basicPropertiesSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.BasicPublish.BasicProperites.Persistent:
						basicProperitesConfig.Persistent = childSection.Value.ToBoolean(); ;
						break;
					default:
						break;
				}
			}
			if (hasChildren) return basicProperitesConfig;
			return null;
		}
		#endregion

		#region IBasicProperitesConfig interface

		bool? IBasicProperitesConfig.Persistent => Persistent;

		#endregion
	}

	public class ConfirmSelectConfig : IConfirmSelectConfig
	{
		#region Public
		public virtual bool Enabled { get; set; } = false;
		public virtual int TimeoutInMs { get; set; } = 2 * 60 * 1000; // 2 minutes

		public static ConfirmSelectConfig Initialize(IConfigurationSection confirmSelectSection)
		{
			if (confirmSelectSection == null) return null;

			bool hasChildren = false;

			ConfirmSelectConfig confirmSelectConfig = new ConfirmSelectConfig();
			foreach (IConfigurationSection childSection in confirmSelectSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.ConfirmSelect.Enabled:
						confirmSelectConfig.Enabled = childSection.Value.ToBoolean(); ;
						break;
					case RabbitMQJsonNames.RabbitMQ.ConfirmSelect.TimeoutInMs:
						confirmSelectConfig.TimeoutInMs = int.Parse(childSection.Value);
						break;
					default:
						break;
				}
			}
			if (hasChildren) return confirmSelectConfig;
			return null;
		}
		#endregion

		#region IBasicProperitesConfig interface

		bool? IConfirmSelectConfig.Enabled => Enabled;
		int? IConfirmSelectConfig.TimeoutInMs => TimeoutInMs;

		#endregion
	}

	public class BasicQosConfig : IBasicQosConfig
	{
		#region Public
		public virtual uint PrefetchSize { get; set; } = 0;
		public virtual ushort PrefetchCount { get; set; } = 1;
		public virtual bool Global { get; set; } = false;

		public static BasicQosConfig Initialize(IConfigurationSection basicQosConfigSection)
		{
			if (basicQosConfigSection == null) return null;

			bool hasChildren = false;

			BasicQosConfig basicQosConfig = new BasicQosConfig();
			foreach (IConfigurationSection childSection in basicQosConfigSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.BasicQos.Global:
						basicQosConfig.Global = childSection.Value.ToBoolean(); ;
						break;
					case RabbitMQJsonNames.RabbitMQ.BasicQos.PrefetchCount:
						basicQosConfig.PrefetchCount = ushort.Parse(childSection.Value);
						break;
					case RabbitMQJsonNames.RabbitMQ.BasicQos.PrefetchSize:
						basicQosConfig.PrefetchSize = uint.Parse(childSection.Value);
						break;
					default:
						break;
				}
			}
			if (hasChildren) return basicQosConfig;
			return null;
		}
		#endregion

		#region IBasicProperitesConfig interface

		uint? IBasicQosConfig.PrefetchSize => PrefetchSize;

		ushort? IBasicQosConfig.PrefetchCount => PrefetchCount;

		bool? IBasicQosConfig.Global => Global;

		#endregion
	}

	public class BasicConsumeConfig : IBasicConsumeConfig
	{
		#region Public
		public virtual bool AutoAck { get; set; } = true;

		public static BasicConsumeConfig Initialize(IConfigurationSection basicConsumeConfigSection)
		{
			if (basicConsumeConfigSection == null) return null;

			bool hasChildren = false;

			BasicConsumeConfig basicConsumeConfig = new BasicConsumeConfig();
			foreach (IConfigurationSection childSection in basicConsumeConfigSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.BasicConsume.AutoAck:
						basicConsumeConfig.AutoAck = childSection.Value.ToBoolean();
						break;
					default:
						break;
				}
			}
			if (hasChildren) return basicConsumeConfig;
			return null;
		}
		#endregion

		#region IBasicProperitesConfig interface

		bool? IBasicConsumeConfig.AutoAck => AutoAck;

		#endregion
	}

	public class TestConfig : ITestConfig
	{
		#region Public
		public virtual bool SendTestMessageOnStart { get; set; } = false;
		public virtual string TestMessage { get; set; }
		public virtual string RoutingKey { get; set; }

		public static TestConfig Initialize(IConfigurationSection testSection)
		{
			if (testSection == null) return null;

			bool hasChildren = false;
			TestConfig testConfig = new TestConfig();
			foreach (IConfigurationSection childSection in testSection.GetChildren())
			{
				hasChildren = true;
				switch (childSection.Key)
				{
					case RabbitMQJsonNames.RabbitMQ.Test.SendTestMessageOnStart:
						testConfig.SendTestMessageOnStart = childSection.Value.ToBoolean(); ;
						break;
					case RabbitMQJsonNames.RabbitMQ.Test.TestMessage:
						testConfig.TestMessage = childSection.Value;
						break;
					case RabbitMQJsonNames.RabbitMQ.Test.RoutingKey:
						testConfig.RoutingKey = childSection.Value;
						break;
					default:
						break;
				}
			}
			if (hasChildren) return testConfig;
			return null;
		}
		#endregion

		#region ITestConfig interface

		bool? ITestConfig.SendTestMessageOnStart => SendTestMessageOnStart;
		string ITestConfig.TestMessage => TestMessage;
		string ITestConfig.RoutingKey => RoutingKey;

		#endregion
	}
}
