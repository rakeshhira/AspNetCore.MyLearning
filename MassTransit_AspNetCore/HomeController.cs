using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MassTransit_AspNetCore
{
	// https://ngohungphuc.wordpress.com/2018/07/22/masstransit-with-asp-net-core-2-1/
	public class Message
	{
		public string Value { get; set; }
	}

	public class SendMessageConsumer : IConsumer<Message>
	{
		Task IConsumer<Message>.Consume(ConsumeContext<Message> context)
		{
			Console.WriteLine($"Received message value: {context.Message.Value}");
			return Task.CompletedTask;
		}
	}

	public class BusService : IHostedService
	{
		private readonly IBusControl _busControl;
		private readonly ILogger<BusService> _logger;

		public BusService(ILogger<BusService> logger, IBusControl busControl)
		{
			_logger = logger;
			_busControl = busControl;
		}

		Task IHostedService.StartAsync(CancellationToken cancellationToken)
		{
			return _busControl.StartAsync(cancellationToken);
		}

		Task IHostedService.StopAsync(CancellationToken cancellationToken)
		{
			return _busControl.StopAsync(cancellationToken);
		}
	}

	[Route("api/[controller]")]
	public class HomeController : Controller
	{
		private readonly IBus _bus;
		private readonly ILogger<HomeController> _logger;

		public HomeController(ILogger<HomeController> logger, IBus bus)
		{
			_logger = logger;
			_bus = bus;
		}

		// GET: api/<controller>
		[HttpGet]
		public async Task<IEnumerable<string>> Get()
		{
			var message = new Message { Value = DateTime.Now.ToString() };
			_logger.LogTrace($"Publishing: {message}");

			await _bus.Publish<Message>(message);

			return new string[] { "value1", "value2" };
		}
	}
}
