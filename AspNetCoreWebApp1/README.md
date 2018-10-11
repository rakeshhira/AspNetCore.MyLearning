# Integrate RabbitMQ

RabbitMQ was integrated with AspNetCoreWebApi1 project based upon lessons learned and captured in [solution README.md](https://github.com/rakeshhira/AspNetCore.MyLearning/)

- [HL7 FHIR Core Support API for STU3](https://www.nuget.org/packages/Hl7.Fhir.STU3)
- [HL7 FHIR Getting Started](http://docs.simplifier.net/fhirnetapi/index.html)
- [Publicly_Available_FHIR_Servers_for_testing](http://wiki.hl7.org/index.php?title=Publicly_Available_FHIR_Servers_for_testing)
- [HAPI FHIR Test Server Observation](http://hapi.fhir.org/baseDstu3/Observation)
- [ORIDASHI FHIR Test Server Observation](http://demo.oridashi.com.au:8297/observation?_format=json)


Specifically following was done to integrate rabbitmq

- a new empty api controllercalled 'RabbitMQController' was added with one methods called 'PublishFhir'
```
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreWebApp1.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RabbitMQController : ControllerBase
	{
		public RabbitMQController()
		{
		}

		[HttpPost]
		public void PublishFhir([FromBody] dynamic fhirResource)
		{
		}
	}
}
```
- [Class is annotated with ApiController attribute](https://docs.microsoft.com/en-us/aspnet/core/web-api/index?view=aspnetcore-2.1#annotate-class-with-apicontrollerattribute)
- Method is declared with `dynamic` instead of `string` or `JObject` to [receive JSON input from body](https://stackoverflow.com/questions/31952002/asp-net-core-mvc-how-to-get-raw-json-bound-to-a-string-without-a-type).
- Added nuget package [Hl7.Fhir.STU3](https://www.nuget.org/packages/Hl7.Fhir.STU3/) 
- Used the [Getting Started](http://docs.simplifier.net/fhirnetapi/index.html) guide and [Parsing and Serialization](http://docs.simplifier.net/fhirnetapi/parsing/poco-parsing.html) guide to parse the 

