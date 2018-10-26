using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit_Messages;

namespace MassTransit_Sender
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Sender> Hello World!");

			var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
			{
				var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
				{
					h.Username("rabbitmq");
					h.Password("rabbitmq");
				});

				sbc.Publish<IHelloMessage>(x =>
				{
					x.AutoDelete = true;
					x.Durable = false;
				});

				//sbc.ReceiveEndpoint(host, "hello_message_queue", ep =>
				//{
				//	ep.AutoDelete = true;
				//	ep.Durable = false;

				//	//ep.Consumer(() => new HelloMessageFaultConsumer());
				//});
			});

			bus.Start();

			var message = new HelloMessage { Text = "Hi" };
			// Publish creates following exchanges
			// (x) HelloMessage
			bus.Publish<IHelloMessage>(message);

			Console.WriteLine($"Published '{message.Text}'");
			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

			bus.Stop();
		}
	}

	public class HelloMessage : IHelloMessage
	{
		public string Text { get; set; } = string.Empty;
		string IHelloMessage.Text => Text;
	}

	public class HelloMessageFaultConsumer : IConsumer<Fault<IHelloMessage>>
	{
		public Task Consume(ConsumeContext<Fault<IHelloMessage>> context)
		{
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("The message was not processed:");
			Console.WriteLine(context.Message.Message.Text);
			Console.WriteLine($"FaultedMessageId={context.Message.FaultedMessageId}");
			Console.WriteLine($"Timestamp={context.Message.Timestamp}");
			Console.ResetColor();
			return Task.CompletedTask;
		}
	}
}
