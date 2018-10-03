using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using TutorialCommon;

namespace Tutorial4_EmitLogDirect
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
					channel.ExchangeDeclare(exchange: "direct_logs",
											type: "direct");

					var severity = (args.Length > 0) ? args[0] : "info";
					var message = (args.Length > 1)
								  ? string.Join(" ", args.Skip(1).ToArray())
								  : "Hello World!";
					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "direct_logs",
										 routingKey: severity,
										 basicProperties: null,
										 body: body);
					Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
				}

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();
			}
			else
			{
				RabbitMQClientMgr rabbitMQClientMgr = TutorialCommonHelper.Initialize();

				var severity = (args.Length > 0) ? args[0] : "info";
				var message = (args.Length > 1)
							  ? string.Join(" ", args.Skip(1).ToArray())
							  : "Hello World!";
				var body = Encoding.UTF8.GetBytes(message);
				rabbitMQClientMgr.BasicPublish(severity, body);

				Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();

				rabbitMQClientMgr.Uninitialize();
			}
		}
	}
}
