using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper;
using RabbitMQ.Client.Wrapper.Abstractions;

namespace AspNetCoreWebApp3.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FhirController : ControllerBase
	{
		readonly ILogger _logger = null;
		readonly IRabbitMQClientWrapper _rabbitMQClientWarpper = null;
		readonly IRabbitMQConfig _rabbitMQConfig = null;

		public FhirController(ILogger<FhirController> logger, IRabbitMQConfig rabbitMQConfig, IRabbitMQClientWrapper rabbitMQClientWarpper)
		{
			_logger = logger;
			_rabbitMQConfig = rabbitMQConfig;
			_rabbitMQClientWarpper = rabbitMQClientWarpper;
		}

		[HttpGet]
		public ActionResult<string> GetLastReceivedMessage()
		{
			return RabbitMQConsumer.LastReceivedMessage;
		}
	}
}