using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace TutorialCommon
{
	public class RabbitMQConfig : IRabbitMQConfig
	{
		#region Private

		private ILogger _logger = null;

		#endregion

		#region Public 
		public virtual ConnectionConfig Connection { get; set; } = null;
		public virtual ExchangeConfig Exchange { get; set; } = null;
		public virtual QueueConfig Queue { get; set; } = null;
		public virtual QueueBindingConfig QueueBinding { get; set; } = null;
		public virtual BasicPublishConfig BasicPublishConfig { get; set; } = null;
		public virtual BasicQosConfig BasicQosConfig { get; set; } = null;
		public virtual BasicConsumeConfig BasicConsumeConfig { get; set; } = null;

		public RabbitMQConfig(ILogger<RabbitMQConfig> logger)
		{
			_logger = logger;
			_logger.LogInformation($"{nameof(RabbitMQConfig)} constructed");
		}

		public virtual void Initialize(IConfigurationSection rabbitMQSection)
		{
			IConfigurationSection connectionSection = rabbitMQSection.GetSection(ConnectionConfigNames.Section);
			Connection = ConnectionConfig.Initialize(connectionSection);

			IConfigurationSection exchangeSection = rabbitMQSection.GetSection(ExchangeConfigNames.Section);
			Exchange = ExchangeConfig.Initialize(exchangeSection);

			IConfigurationSection queueSection = rabbitMQSection.GetSection(QueueConfigNames.Section);
			Queue = QueueConfig.Initialize(queueSection);

			IConfigurationSection queueBindingSection = rabbitMQSection.GetSection(QueueBindingConfigNames.Section);
			QueueBinding = QueueBindingConfig.Initialize(queueBindingSection);

			IConfigurationSection basicPublishConfigSection = rabbitMQSection.GetSection(BasicPublishConfigNames.Section);
			BasicPublishConfig = BasicPublishConfig.Initialize(basicPublishConfigSection);

			IConfigurationSection basicQosConfigSection = rabbitMQSection.GetSection(BasicQosConfigNames.Section);
			BasicQosConfig = BasicQosConfig.Initialize(basicQosConfigSection);

			IConfigurationSection basicConsumeConfigSection = rabbitMQSection.GetSection(BasicConsumeConfigNames.Section);
			BasicConsumeConfig = BasicConsumeConfig.Initialize(basicConsumeConfigSection);
		}

		#endregion

		#region IRabbitMQConfig Interface
		IConnectionConfig IRabbitMQConfig.Connection => Connection;
		IExchangeConfig IRabbitMQConfig.Exchange => Exchange;
		IQueueConfig IRabbitMQConfig.Queue => Queue;
		IQueueBindingConfig IRabbitMQConfig.QueueBinding => QueueBinding;
		IBasicPublishConfig IRabbitMQConfig.BasicPublishConfig => BasicPublishConfig;
		IBasicQosConfig IRabbitMQConfig.BasicQosConfig => BasicQosConfig;
		IBasicConsumeConfig IRabbitMQConfig.BasicConsumeConfig => BasicConsumeConfig;
		void IRabbitMQConfig.Initialize(IConfigurationSection configurationSection)
		{
			Initialize(configurationSection);
		}

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
					case ConnectionConfigNames.HostName:
						connectionConfig.HostName = childSection.Value;
						break;
					case ConnectionConfigNames.UserName:
						connectionConfig.UserName = childSection.Value;
						break;
					case ConnectionConfigNames.Password:
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
					case ExchangeConfigNames.Name:
						exchangeConfig.Name = childSection.Value;
						break;
					case ExchangeConfigNames.Type:
						exchangeConfig.Type = childSection.Value;
						break;
					case ExchangeConfigNames.Durable:
						exchangeConfig.Durable = DotNetCoreHelper.BoolParse(childSection.Value);
						break;
					case ExchangeConfigNames.AutoDelete:
						exchangeConfig.AutoDelete = DotNetCoreHelper.BoolParse(childSection.Value);
						break;
					case ExchangeConfigNames.Arguments:
						exchangeConfig.AutoDelete = DotNetCoreHelper.BoolParse(childSection.Value);
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
					case QueueConfigNames.Name:
						queueConfig.Name = childSection.Value;
						break;
					case QueueConfigNames.Exclusive:
						queueConfig.Exclusive = DotNetCoreHelper.BoolParse(childSection.Value);
						break;
					case QueueConfigNames.Durable:
						queueConfig.Durable = DotNetCoreHelper.BoolParse(childSection.Value);
						break;
					case QueueConfigNames.AutoDelete:
						queueConfig.AutoDelete = DotNetCoreHelper.BoolParse(childSection.Value);
						break;
					case QueueConfigNames.Arguments:
						//AutoDelete = DotNetCoreHelper.BoolParse(childSection.Value);
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
					case QueueBindingConfigNames.RoutingKeys:
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
					case BasicProperitesConfigNames.RoutingKey:
						basicPublishConfig.RoutineKey = childSection.Value;
						break;
					case BasicProperitesConfigNames.Section:
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
					case BasicProperitesConfigNames.Persistent:
						basicProperitesConfig.Persistent = DotNetCoreHelper.BoolParse(childSection.Value);
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
					case BasicQosConfigNames.Global:
						basicQosConfig.Global = DotNetCoreHelper.BoolParse(childSection.Value);
						break;
					case BasicQosConfigNames.PrefetchCount:
						basicQosConfig.PrefetchCount = ushort.Parse(childSection.Value);
						break;
					case BasicQosConfigNames.PrefetchSize:
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
					case BasicConsumeConfigNames.AutoAck:
						basicConsumeConfig.AutoAck = DotNetCoreHelper.BoolParse(childSection.Value);
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
}
