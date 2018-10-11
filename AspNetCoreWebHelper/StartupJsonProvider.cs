using System;
using System.IO;
using System.Reflection;
using DotNetCoreCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper;
using RabbitMQ.Client.Wrapper.Abstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCoreWebCommon
{
	public class StartupJsonProvider
	{
		#region JSON Helpers

		public static class JsonNames
		{
			public static class Startup
			{
				public const string Section = "startup";
				public static class Swagger
				{
					public const string Section = "swagger";
					public const string Enabled = "enabled";
					public const string IncludeXmlComments = "includeXmlComments";
					public const string DocName = "docName";
					public const string DocVersion = "docVersion";
					public const string EndPoint = "endPoint";
					public const string RoutePrefix = "routePrefix";
				}
			}
		}

		private IConfigurationSection _startupSection = null;
		private IConfigurationSection StartupSection
		{
			get
			{
				if (_startupSection == null)
				{
					_startupSection = Configuration.GetSection(JsonNames.Startup.Section);
				}
				return _startupSection;
			}
		}

		private IConfigurationSection _swaggerSection = null;
		private IConfigurationSection SwaggerSection
		{
			get
			{
				if (_swaggerSection == null)
				{
					_swaggerSection = StartupSection.GetSection(JsonNames.Startup.Swagger.Section);
				}
				return _swaggerSection;
			}
		}

		public bool EnableSwagger
		{
			get
			{
				IConfigurationSection propSection = SwaggerSection.GetSection(JsonNames.Startup.Swagger.Enabled);
				return propSection.Value.ToBoolean(false);
			}
		}

		public bool SwaggerIncludeXmlComments
		{
			get
			{
				IConfigurationSection propSection = SwaggerSection.GetSection(JsonNames.Startup.Swagger.IncludeXmlComments);
				return propSection.Value.ToBoolean(false);
			}
		}

		public string SwaggerDocName
		{
			get
			{
				IConfigurationSection propSection = SwaggerSection.GetSection(JsonNames.Startup.Swagger.DocName);
				return propSection.Value ?? EntryAssemblyName;
			}
		}

		public string SwaggerDocVersion
		{
			get
			{
				IConfigurationSection propSection = SwaggerSection.GetSection(JsonNames.Startup.Swagger.DocVersion);
				return propSection.Value ?? "V1";
			}
		}

		public string SwaggerEndPoint
		{
			get
			{
				IConfigurationSection propSection = SwaggerSection.GetSection(JsonNames.Startup.Swagger.EndPoint);
				return propSection.Value ?? "/swagger/v1/swagger.json";
			}
		}

		public string SwaggerRoutePrefix
		{
			get
			{
				IConfigurationSection propSection = SwaggerSection.GetSection(JsonNames.Startup.Swagger.RoutePrefix);
				return propSection.Value ?? string.Empty;
			}
		}

		public string EntryAssemblyName
		{
			get
			{
				return Assembly.GetEntryAssembly().GetName().Name;
			}
		}

		public bool EnableRabbitMQ
		{
			get
			{
				IConfigurationSection rabbitMQSecction = Configuration.GetSection(RabbitMQJsonNames.RabbitMQ.Section);
				if (rabbitMQSecction.GetChildren().GetEnumerator().MoveNext())
				{
					return true;
				}
				return false;
			}
		}
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
}
