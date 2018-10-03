using System;
using System.Text;
using RabbitMQ.Client;
using TutorialCommon;

namespace Tutorial2_NewTask
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

					var message = GetMessage(args);
					var body = Encoding.UTF8.GetBytes(message);

					var properties = channel.CreateBasicProperties();
					properties.Persistent = true;

					channel.BasicPublish(exchange: "",
										 routingKey: "task_queue",
										 basicProperties: properties,
										 body: body);
					Console.WriteLine(" [x] Sent {0}", message);
				}

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
			else
			{
				Console.WriteLine("Tutorial2_NewTask> Hello World!");

				RabbitMQClientMgr rabbitMQClientMgr = TutorialCommonHelper.Initialize();

				var message = GetMessage(args);
				var body = Encoding.UTF8.GetBytes(message);
				rabbitMQClientMgr.BasicPublish(
					routingKey: "task_queue",
					body: body);

				Console.WriteLine(" [x] Sent {0}", message);

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();

				rabbitMQClientMgr.Uninitialize();
			}
		}


		private static string GetMessage(string[] args)
		{
			return args.Length > 0 ? string.Join(" ", args) : "Hello World!!";
		}
	}
}
