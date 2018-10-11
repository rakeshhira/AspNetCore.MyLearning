using RabbitMQ.Client.Events;

namespace RabbitMQ.Client.Wrapper.Abstractions
{
	public interface IRabbitMQClientWrapper
	{
		IConnection Connection { get; }

		IModel Model { get; }

		void Initialize();

		void BasicPublish(byte[] body);

		void BasicPublish(string body);

		void BasicPublish(string routingKey, byte[] body);

		void BasicPublish(string routingKey, byte[] body, bool shouldWaitForConfirms, out bool timedOut);

		void QueueBind(string routingKey);

		void BasicConsume();

		void BasicAck(ulong deliveryTag, bool multiple);

		EventingBasicConsumer EventingBasicConsumer { get; }
	}
}
