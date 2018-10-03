namespace TutorialCommon
{
	public static class RabbitMQConfigNames
	{
		public const string Section = "rabbitmq";
	}

	public static class ConnectionConfigNames
	{
		public const string Section = "connection";
		public const string HostName = "hostname";
		public const string UserName = "username";
		public const string Password = "password";
	}

	public static class ExchangeConfigNames
	{
		public const string Section = "exchange";
		public const string Name = "name";
		public const string Type = "type";
		public const string Durable = "durable";
		public const string AutoDelete = "autoDelete";
		public const string Arguments = "arguments";
	}

	public static class QueueConfigNames
	{
		public const string Section = "queue";
		public const string Name = "name";
		public const string Exclusive = "exclusive";
		public const string Durable = "durable";
		public const string AutoDelete = "autoDelete";
		public const string Arguments = "arguments";
	}

	public static class QueueBindingConfigNames
	{
		public const string Section = "queueBinding";
		public const string RoutingKeys = "routingKeys";
	}

	public static class BasicPublishConfigNames
	{
		public const string Section = "basicPublish";
	}

	public static class BasicProperitesConfigNames
	{
		public const string Section = "basicProperties";
		public const string RoutingKey = "routingKey";
		public const string Persistent = "persistent";
	}

	public static class BasicQosConfigNames
	{
		public const string Section = "basicQos";
		public const string PrefetchSize = "prefetchSize";
		public const string PrefetchCount = "prefetchCount";
		public const string Global = "global";
	}

	public static class BasicConsumeConfigNames
	{
		public const string Section = "basicConsume";
		public const string AutoAck = "autoAck";
	}
}
