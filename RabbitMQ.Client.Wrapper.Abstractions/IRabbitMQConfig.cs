﻿using System.Collections.Generic;

namespace RabbitMQ.Client.Wrapper.Abstractions
{
	public interface IRabbitMQConfig
	{
		IConnectionConfig Connection { get; }

		IExchangeConfig Exchange { get; }

		IQueueConfig Queue { get; }

		IQueueBindingConfig QueueBinding { get; }

		IBasicPublishConfig BasicPublishConfig { get; }

		IConfirmSelectConfig ConfirmSelectConfig { get; }

		IBasicQosConfig BasicQosConfig { get; }

		IBasicConsumeConfig BasicConsumeConfig { get; }

		ITestConfig TestConfig { get; }

		void Initialize();
	}

	public interface IConnectionConfig
	{
		string HostName { get; }
		string UserName { get; }
		string Password { get; }
	}

	public interface IExchangeConfig
	{
		string Name { get; }
		string Type { get; }

		bool? Durable { get; }
		bool? AutoDelete { get; }

		IDictionary<string, object> Arguments { get; }
	}

	public interface IQueueConfig
	{
		string Name { get; }

		void SetAutoGeneratedQueueName(string queueName);

		bool? Exclusive { get; }

		bool? Durable { get; }

		bool? AutoDelete { get; }

		IDictionary<string, object> Arguments { get; }
	}

	public interface IQueueBindingConfig
	{
		IList<string> RoutingKeys { get; }
	}

	public interface IBasicPublishConfig
	{
		string RoutingKey { get; }
		IBasicProperitesConfig BasicProperitesConfig { get; }
	}

	public interface IConfirmSelectConfig
	{
		bool? Enabled { get; }
		int? TimeoutInMs { get; }
	}

	public interface IBasicProperitesConfig
	{
		bool? Persistent { get; }
	}
	public interface IBasicQosConfig
	{
		uint? PrefetchSize { get; }
		ushort? PrefetchCount { get; }
		bool? Global { get; }
	}

	public interface IBasicConsumeConfig
	{
		bool? AutoAck { get; }
	}

	public interface ITestConfig
	{
		bool? SendTestMessageOnStart { get; }

		string TestMessage { get; }

		string RoutingKey { get; }
	}
}
