using AspNetCoreWebCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper;

namespace AspNetCoreWebApp3
{
	public class Startup : StartupJsonProvider
	{
		public Startup(IConfiguration configuration, ILoggerFactory loggerFactory)
			: base(configuration, loggerFactory)
		{
		}

		// This method gets called by the runtime. Use this method to add services to the container.
		override public void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			if (EnableRabbitMQ)
			{
				services.AddSingleton<RabbitMQConsumer, RabbitMQConsumer>();
			}
		}

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
		}
	}
}
