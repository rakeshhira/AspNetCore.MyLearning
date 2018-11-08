using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AspNetCoreWebApp2.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FhirController : ControllerBase
	{
		readonly ILogger _logger = null;
		readonly IBus _bus = null;

		public FhirController(ILogger<FhirController> logger, IBus bus)
		{
			_logger = logger;
			_bus = bus;
		}

		[HttpGet]
		public ActionResult<string> GetLastReceivedMessage()
		{
			return "Not Implemented";
		}
	}
}