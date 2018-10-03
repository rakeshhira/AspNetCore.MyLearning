using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace NetCoreConsoleAppWorker2
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine($"{Process.GetCurrentProcess().Id} > Worker2> Hello World!");

			var factory = new ConnectionFactory();
			factory.HostName = "localhost";
			factory.UserName = "rabbitmq";
			factory.Password = "rabbitmq";
			using (var connection = factory.CreateConnection())
			{
				using (var channel = connection.CreateModel())
				{
					// receiver may start before consumer, so queue is declared here too
					channel.QueueDeclare(queue: "task_queue",
						durable: false,
						exclusive: false,
						autoDelete: false,
						arguments: null);

					// Experimental class exposing an IBasicConsumer's methods as separate events.
					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += OnMessageReceived;

					// Start a Basic content-class consumer.
					channel.BasicConsume(queue: "task_queue",
						autoAck: false,
						consumer: consumer);

					Console.WriteLine(" Press [enter] to quit...");
					Console.ReadLine();
				}
			}
		}

		private static void OnMessageReceived(object sender, BasicDeliverEventArgs basicDeliverEventArgs)
		{
			var body = basicDeliverEventArgs.Body;
			var message = Encoding.UTF8.GetString(body);
			Console.WriteLine($" [x] Received {message}");

			int dots = message.Split('.').Length - 1;
			Thread.Sleep(dots * 1000);

			Console.WriteLine(" [x] Done");

			var eventingBasicConsumer = sender as EventingBasicConsumer;

			eventingBasicConsumer.Model.BasicAck(deliveryTag: basicDeliverEventArgs.DeliveryTag, multiple: false);

			Console.WriteLine(" Waiting...");
			Console.WriteLine(" Press [enter] to quit...");
		}
	}
}
