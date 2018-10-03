using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Tutorial3_ReceiveLogs
{
	class Program
	{
		static void Main(string[] args)
		{
			bool usingTutorialCommonHelper = false;
			if (!usingTutorialCommonHelper)
			{
				var factory = new ConnectionFactory() { HostName = "localhost", UserName = "rabbitmq", Password = "rabbitmq" };
				using (var connection = factory.CreateConnection())
				using (var channel = connection.CreateModel())
				{
					channel.ExchangeDeclare(exchange: "logs", type: "fanout");

					var queueName = channel.QueueDeclare().QueueName;
					channel.QueueBind(queue: queueName,
						exchange: "logs",
						routingKey: "");

					Console.WriteLine(" [*] Waiting for logs");

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += OnMessageReceived;

					channel.BasicConsume(queue: queueName,
						autoAck: true,
						consumer: consumer);

					Console.WriteLine(" Press [enter] to exit");
					Console.ReadLine();
				}
			}
		}

		private static void OnMessageReceived(object sender, BasicDeliverEventArgs basicDeliverEventArgs)
		{
			var body = basicDeliverEventArgs.Body;
			string message = Encoding.UTF8.GetString(body);
			Console.WriteLine($" [x] Received {message}");
		}
	}
}
