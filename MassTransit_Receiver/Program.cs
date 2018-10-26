using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit_Messages;

namespace MassTransit_Receive
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Receiver> Hello World!");

			var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
			{
				var host = sbc.Host(new Uri("rabbitmq://localhost"), h =>
				{
					h.Username("rabbitmq");
					h.Password("rabbitmq");
				});

				sbc.ReceiveEndpoint(host, "your_message_queue", e =>
				{
					e.Consumer(() => new HelloMessageConsumer());
				});
			});

			bus.Start();

			Console.WriteLine("Press any key to exit");
			Console.ReadKey();

			bus.Stop();
		}
	}
	public class HelloMessageConsumer : IConsumer<IHelloMessage>
	{
		public async Task Consume(ConsumeContext<IHelloMessage> context)
		{
			await Console.Out.WriteLineAsync($"Updating customer: {context.Message.Text}");

			// update the customer address
		}
	}
}
