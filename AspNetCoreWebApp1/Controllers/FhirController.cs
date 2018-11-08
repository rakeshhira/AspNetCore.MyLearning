using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace AspNetCoreWebApp1.Controllers
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

		[HttpPost]
		public void Publish([FromBody] dynamic fhirResourceJObect)
		{
			JObject jObject = fhirResourceJObect as JObject;
			_logger.LogDebug($"{nameof(Publish)} called.  {nameof(fhirResourceJObect)}={fhirResourceJObect}");

			_bus.Publish<JObject>(jObject);
		}
	}
}