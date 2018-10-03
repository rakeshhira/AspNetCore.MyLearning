using System;
using System.Text;
using RabbitMQ.Client;

namespace Tutorial3_Emit
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
				using (var model = connection.CreateModel())
				{
					model.ExchangeDeclare(exchange: "logs", type: "fanout");

					var message = GetMessage(args); ;
					var body = Encoding.UTF8.GetBytes(message);
					model.BasicPublish(exchange: "logs",
						routingKey: "",
						basicProperties: null,
						body: body);
					Console.WriteLine($" [x] Send {message}");
				}

				Console.WriteLine(" Press [enter] to exit");
				Console.ReadLine();
			}
			else
			{

			}
		}

		private static string GetMessage(string[] args)
		{
			return args.Length > 0 ? string.Join(" ", args) : "Pub > Hello World!!";
		}
	}
}
