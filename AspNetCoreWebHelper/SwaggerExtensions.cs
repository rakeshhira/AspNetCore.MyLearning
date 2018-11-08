using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;

namespace AspNetCoreWebCommon
{
	public static class SwaggerExtensions
	{
		public static IServiceCollection AddSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			var swaggerOptions = configuration.GetSection("Swagger").Get<SwaggerOptions>();
			if (swaggerOptions.Enabled)
			{
				services.AddSwaggerGen(c =>
				{
					string entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
					var docName = swaggerOptions.DocName ?? entryAssemblyName;

					var docVersion = swaggerOptions.DocVersion ?? "V1";
					c.SwaggerDoc("v1", new Info { Title = docName, Version = docVersion });

					if (swaggerOptions.IncludeXmlComments)
					{
						var xmlPath = Path.Combine(AppContext.BaseDirectory, $"{entryAssemblyName}.xml");
						c.IncludeXmlComments(xmlPath);
					}
				});
			}

			return services;
		}

		public static IApplicationBuilder UseSwagger(this IApplicationBuilder app, IConfiguration configuration)
		{
			var swaggerOptions = configuration.GetSection("Swagger").Get<SwaggerOptions>();
			if (swaggerOptions.Enabled)
			{
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint(swaggerOptions.EndPoint, $"{swaggerOptions.DocName} {swaggerOptions.DocVersion}");
					c.RoutePrefix = swaggerOptions.RoutePrefix;
				});
			}

			return app;
		}
	}
}
