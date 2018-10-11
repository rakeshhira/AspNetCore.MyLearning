# Integrate RabbitMQ

RabbitMQ was integrated with AspNetCoreWebApi1 project based upon lessons learned and captured in [solution README.md](https://github.com/rakeshhira/AspNetCore.MyLearning/)

- [HL7 FHIR Core Support API for STU3](https://www.nuget.org/packages/Hl7.Fhir.STU3)
- [HL7 FHIR Getting Started](http://docs.simplifier.net/fhirnetapi/index.html)
- [Publicly_Available_FHIR_Servers_for_testing](http://wiki.hl7.org/index.php?title=Publicly_Available_FHIR_Servers_for_testing)
- [HAPI FHIR Test Server Observation](http://hapi.fhir.org/baseDstu3/Observation)
- [ORIDASHI FHIR Test Server Observation](http://demo.oridashi.com.au:8297/observation?_format=json)

Specifically following was done to integrate rabbitmq

- I recreated "Tutorial1_Sender", "Tutorial1_Receiver", "Tutorial5_EmitLogTopic", "Tutorial5_ReceiveLogsTopic" as ASP.Net Core web applications "AspNetCoreWeb_Sender", "AspNetCoreWeb_Receiver", "AspNetCoreWeb_EmitLogTopic", "AspNetCoreWeb_ReceiveLogsTopic".  The projects were created using "empty" Asp.Net Core Web project template to keep the asp.net specific code to minimal.
- The RabbitMQ + Swagger settings were moved into appsettings.json file.  
- appsettings.json for AspNetCoreWeb_Sender is as following
```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information"
		}
	},
	"AllowedHosts": "*",
	"rabbitmq": {
		"connection": {
			"hostname": "localhost",
			"username": "rabbitmq",
			"password": "rabbitmq"
		},
		"queue": {
			"name": "hello",
			"exclusive": "0",
			"durable": "0",
			"autoDelete": "1",
			"arguments": []
		},
		"basicPublish": {
			"routingKey": "hello"
		},
		"test": {
			"sendTestMessageOnStart": "true",
			"testMessage": "hello world!",
			"routingKey": "hello"
		}
	}
}
```
- appsettings.json for AspNetCoreWeb_Receiver is as following
```json
{
	"Logging": {
		"LogLevel": {
			"Default": "Information"
		}
	},
	"AllowedHosts": "*",
	"rabbitmq": {
		"connection": {
			"hostname": "localhost",
			"username": "rabbitmq",
			"password": "rabbitmq"
		},
			"queue": {
				"name": "hello",
				"exclusive": "0",
				"durable": "0",
				"autoDelete": "1",
				"arguments": []
			}
	}
}
```
- Initializing RabbitMQ + Swagger from appsettings.json was consolidated under StartupJsonProvider.cs class in AspNetCoreWebCommon project. 
```csharp
	public class StartupJsonProvider
	{
		#region JSON Helpers
		...
		#endregion

		public StartupJsonProvider(IConfiguration configuration, ILoggerFactory loggerFactory)
		{
			Configuration = configuration;
			_loggerFactory = loggerFactory;
		}

		public IConfiguration Configuration { get; }

		private ILoggerFactory _loggerFactory { get; }

		public IServiceCollection ServiceCollection { get; private set; }


		virtual public void ConfigureServices(IServiceCollection services)
		{
			var logger = _loggerFactory.CreateLogger<StartupJsonProvider>();

			ServiceCollection = services;
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

			if (EnableRabbitMQ)
			{
				logger.LogInformation("Configuring RabbitMQ services");
				services.AddSingleton<IRabbitMQConfig, RabbitMQConfig>();
				services.AddSingleton<IRabbitMQClientWrapper, RabbitMQClientWrapper>();
			}

			if (EnableSwagger)
			{
				logger.LogInformation("Configuring Swagger servies");
				services.AddSwaggerGen(c =>
				{
					c.SwaggerDoc("v1", new Info { Title = SwaggerDocName, Version = SwaggerDocVersion });

					if (SwaggerIncludeXmlComments)
					{
						var xmlFile = $"{EntryAssemblyName}.xml";
						var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
						c.IncludeXmlComments(xmlPath);
					}
				});
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		virtual public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			var logger = _loggerFactory.CreateLogger<StartupJsonProvider>();
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			// Enable middleware to serve generated Swagger as a JSON endpoint.
			if (EnableSwagger)
			{
				logger.LogInformation("Enabling Swagger middleware");
				app.UseSwagger();

				// Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
				// specifying the Swagger JSON endpoint.
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint(SwaggerEndPoint, $"{SwaggerDocName} {SwaggerDocVersion}");
					c.RoutePrefix = SwaggerRoutePrefix;
				});
			}

			if (EnableRabbitMQ)
			{
				logger.LogInformation("Registering RabbitMQ with application lifetime");
				RabbitMQConfig rabbitMQConfig = app.ApplicationServices.GetRequiredService<IRabbitMQConfig>() as RabbitMQConfig;
				RabbitMQClientWrapper rabbitMQClientWrapper = app.ApplicationServices.GetRequiredService<IRabbitMQClientWrapper>() as RabbitMQClientWrapper;

				var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
				applicationLifetime.ApplicationStarted.Register(() =>
				{
					rabbitMQConfig.Initialize();
					rabbitMQClientWrapper.Initialize();
				});
				applicationLifetime.ApplicationStopped.Register(() =>
				{
					rabbitMQClientWrapper.Uninitialize();
				});
			}

			app.UseMvc();
		}
	}
```
- I intergrated RabbitMQ into AspNetCoreWebApp1, AspNetCoreWebApp2 & AspNetCoreWebApp3 based upon the learning from tutorials.  I added API controller to receive FHIR data.
- The FHIRController just had one method that took FHIR as input.  **NOTE** the input variable was declared as `dynamic` instead of `string`.  Declaring it 'string' was not working.  See [receive JSON input from body](https://stackoverflow.com/questions/31952002/asp-net-core-mvc-how-to-get-raw-json-bound-to-a-string-without-a-type).
- FHIR support was added via nuget package [Hl7.Fhir.STU3](https://www.nuget.org/packages/Hl7.Fhir.STU3/) See [Getting Started](http://docs.simplifier.net/fhirnetapi/index.html) guide and [Parsing and Serialization](http://docs.simplifier.net/fhirnetapi/parsing/poco-parsing.html) guide.
