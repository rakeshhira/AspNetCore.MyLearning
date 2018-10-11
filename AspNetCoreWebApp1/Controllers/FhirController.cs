using DotNetCoreCommon;
using Hl7.Fhir.ElementModel;
using Hl7.Fhir.Model;
using Hl7.Fhir.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client.Wrapper.Abstractions;

namespace AspNetCoreWebApp1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class FhirController : ControllerBase
	{
		readonly ILogger _logger = null;
		readonly IRabbitMQClientWrapper _rabbitMQClientWarpper = null;
		const string FhirDataType = "fhir";

		public FhirController(ILogger<FhirController> logger, IRabbitMQClientWrapper rabbitMQClientWarpper)
		{
			_logger = logger;
			_rabbitMQClientWarpper = rabbitMQClientWarpper;
		}

		[HttpPost]
		public void Publish([FromBody] dynamic fhirResourceJObect)
		{
			JObject jObject = fhirResourceJObect as JObject;
			_logger.LogDebug($"{nameof(Publish)} called.  {nameof(fhirResourceJObect)}={fhirResourceJObect}");

			IElementNavigator elementNavigator = JsonDomFhirNavigator.Create(jObject);
			var fhirParser = new FhirJsonParser();

			JToken resourceTypeJToken = jObject["resourceType"];
			Resource resource = null;
			switch (resourceTypeJToken.ToString())
			{
				case "Bundle":
					resource = fhirParser.Parse<Bundle>(elementNavigator);
					break;
				case "Patient":
					resource = fhirParser.Parse<Patient>(elementNavigator);
					break;
				case "observation":
					resource = fhirParser.Parse<Observation>(elementNavigator);
					break;
				default:
					break;
			}
			string topic = BuildTopic(resource);
			_rabbitMQClientWarpper.BasicPublish(topic, jObject.ToString().ToUtf8Bytes());
		}

		private string BuildTopic(Resource resource)
		{
			return $"{FhirDataType}.{resource.ResourceType.ToString()}";
		}
	}
}