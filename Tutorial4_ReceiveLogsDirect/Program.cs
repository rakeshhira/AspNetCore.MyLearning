using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using TutorialCommon;

namespace Tutorial4_ReceiveLogsDirect
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
					var queueName = channel.QueueDeclare().QueueName;

					if (args.Length < 1)
					{
						Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
												Environment.GetCommandLineArgs()[0]);
						Console.WriteLine(" Press [enter] to exit.");
						Console.ReadLine();
						Environment.ExitCode = 1;
						return;
					}

					foreach (var severity in args)
					{
						channel.QueueBind(queue: queueName,
										  exchange: "direct_logs",
										  routingKey: severity);
					}

					Console.WriteLine(" [*] Waiting for messages.");

					var consumer = new EventingBasicConsumer(channel);
					consumer.Received += (model, ea) =>
					{
						var body = ea.Body;
						var message = Encoding.UTF8.GetString(body);
						var routingKey = ea.RoutingKey;
						Console.WriteLine(" [x] Received '{0}':'{1}'",
										  routingKey, message);
					};
					channel.BasicConsume(queue: queueName,
										 autoAck: true,
										 consumer: consumer);

					Console.WriteLine(" Press [enter] to exit.");
					Console.ReadLine();
				}
			}
			else
			{
				RabbitMQClientMgr rabbitMQClientMgr = TutorialCommonHelper.Initialize(args.ToList());

				Console.WriteLine(" [*] Waiting for messages.");

				rabbitMQClientMgr.EventingBasicConsumer.Received += (model, ea) =>
				{
					var body = ea.Body;
					var message = Encoding.UTF8.GetString(body);
					var routingKey = ea.RoutingKey;
					Console.WriteLine(" [x] Received '{0}':'{1}'",
									  routingKey, message);
				};

				rabbitMQClientMgr.BasicConsume();

				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();

				rabbitMQClientMgr.Uninitialize();
			}
		}
	}
}
