namespace RabbitMQ.Client.Wrapper.Abstractions
{
	public static class RabbitMQJsonNames
	{
		public static class RabbitMQ
		{
			public const string Section = "rabbitmq";

			public static class Connection
			{
				public const string Section = "connection";
				public const string HostName = "hostname";
				public const string UserName = "username";
				public const string Password = "password";
			}

			public static class Exchange
			{
				public const string Section = "exchange";
				public const string Name = "name";
				public const string Type = "type";
				public const string Durable = "durable";
				public const string AutoDelete = "autoDelete";
				public const string Arguments = "arguments";
			}

			public static class Queue
			{
				public const string Section = "queue";
				public const string Name = "name";
				public const string Exclusive = "exclusive";
				public const string Durable = "durable";
				public const string AutoDelete = "autoDelete";
				public const string Arguments = "arguments";
			}

			public static class QueueBinding
			{
				public const string Section = "queueBinding";
				public const string RoutingKeys = "routingKeys";
			}

			public static class BasicPublish
			{
				public const string Section = "basicPublish";
				public const string RoutingKey = "routingKey";

				public static class BasicProperites
				{
					public const string Section = "basicProperties";
					public const string Persistent = "persistent";
				}
			}

			public static class ConfirmSelect
			{
				public const string Section = "confirmSelect";
				public const string Enabled = "enabled";
				public const string TimeoutInMs = "timeoutInMs";
			}


			public static class BasicQos
			{
				public const string Section = "basicQos";
				public const string PrefetchSize = "prefetchSize";
				public const string PrefetchCount = "prefetchCount";
				public const string Global = "global";
			}

			public static class BasicConsume
			{
				public const string Section = "basicConsume";
				public const string AutoAck = "autoAck";
			}

			public static class Test
			{
				public const string Section = "test";
				public const string SendTestMessageOnStart = "sendTestMessageOnStart";
				public const string TestMessage = "testMessage";
				public const string RoutingKey = "routingKey";
			}
		}
	}

}
