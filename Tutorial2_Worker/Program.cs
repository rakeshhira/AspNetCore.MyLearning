using System;
using System.Text;
using System.Threading;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TutorialCommon;

namespace Tutorial2_Worker
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
					channel.QueueDeclare(queue: "task_queue",
										 durable: true,
										 exclusive: false,
										 autoDelete: false,
										 arguments: null);

					channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

					Console.WriteLine(" [*] Waiting for messages.");

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (model, ea) =>
					{
						var body = ea.Body;
						var message = Encoding.UTF8.GetString(body);
						Console.WriteLine(" [x] Received {0}", message);

						int dots = message.Split('.').Length - 1;
						Thread.Sleep(dots * 1000);

						Console.WriteLine(" [x] Done");

						channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
					};
					channel.BasicConsume(queue: "task_queue",
										 autoAck: false,
										 consumer: consumer);

					Console.WriteLine(" Press [enter] to exit.");
					Console.ReadLine();
				}
			}
			else
			{
				Console.WriteLine("Tutorial2_Worker> Hello World!");

				RabbitMQClientMgr rabbitMQClientMgr = TutorialCommonHelper.Initialize();

				Console.WriteLine(" [*] Waiting for messages.");

				rabbitMQClientMgr.EventingBasicConsumer.Received += (model, ea) =>
				{
					var body = ea.Body;
					var message = Encoding.UTF8.GetString(body);
					Console.WriteLine(" [x] Received {0}", message);

					int dots = message.Split('.').Length - 1;
					Thread.Sleep(dots * 1000);

					Console.WriteLine(" [x] Done");

					rabbitMQClientMgr.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
				};

				rabbitMQClientMgr.BasicConsume();

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();

				rabbitMQClientMgr.Uninitialize();
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
