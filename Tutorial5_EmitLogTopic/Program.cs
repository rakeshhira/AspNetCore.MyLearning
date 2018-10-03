﻿using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;
using TutorialCommon;

namespace Tutorial5_EmitLogTopic
{
	class Program
	{
		static void Main(string[] args)
		{
			bool usingTutorialCommonHelper = true;
			if (!usingTutorialCommonHelper)
			{
				var factory = new ConnectionFactory() { HostName = "localhost", UserName = "rabbitmq", Password = "rabbitmq" };
				using (var connection = factory.CreateConnection())
				using (var channel = connection.CreateModel())
				{
					channel.ExchangeDeclare(exchange: "topic_logs",
						type: "topic");
					var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
					var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
					var body = Encoding.UTF8.GetBytes(message);
					channel.BasicPublish(exchange: "topic_logs",
						routingKey: routingKey,
						basicProperties: null,
						body: body);
					Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");
					Console.WriteLine(" Press [enter] to exit.");
					Console.ReadLine();
				}
			}
			else
			{
				RabbitMQClientMgr rabbitMQClientMgr = TutorialCommonHelper.Initialize();

				var routingKey = (args.Length > 0) ? args[0] : "anonymous.info";
				var message = (args.Length > 1) ? string.Join(" ", args.Skip(1).ToArray()) : "Hello World!";
				var body = Encoding.UTF8.GetBytes(message);
				rabbitMQClientMgr.BasicPublish(routingKey, body);

				Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");
				Console.WriteLine(" Press [enter] to exit.");
				Console.ReadLine();

				rabbitMQClientMgr.Uninitialize();
			}
		}
	}
}
