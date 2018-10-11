using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper.Abstractions;

namespace AspNetCoreWebApp2.Controllers
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
		public ActionResult<string> Status()
		{
			return $"Connection.IsOpen={_rabbitMQClientWarpper.Connection.IsOpen}, Model.IsOpen={_rabbitMQClientWarpper.Model.IsOpen}, _rabbitMQConfig.Queue.Name={_rabbitMQConfig.Queue.Name}, _rabbitMQClientWarpper.Model.MessageCount={_rabbitMQClientWarpper.Model.MessageCount(_rabbitMQConfig.Queue.Name)}";
		}
	}
}