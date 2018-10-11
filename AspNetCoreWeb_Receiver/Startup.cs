using AspNetCoreWebCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper;

namespace AspNetCoreWeb_Receiver
{
	public class Startup : StartupJsonProvider
	{
		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
			: base(configuration, loggerFactory)
		{

		}

		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		override public void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			if (EnableRabbitMQ)
			{
				services.AddSingleton<RabbitMQConsumer, RabbitMQConsumer>();
			}
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		override public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			base.Configure(app, env);

			if (EnableRabbitMQ)
			{
				RabbitMQConsumer rabbitMQConsumer = app.ApplicationServices.GetRequiredService<RabbitMQConsumer>();

				var applicationLifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
				applicationLifetime.ApplicationStarted.Register(() =>
				{
					rabbitMQConsumer.Register();
				});
			}

			app.Run(async (context) =>
			{
				await context.Response.WriteAsync($"{nameof(AspNetCoreWeb_Receiver)} > Hello World!");
			});
		}
	}
}
