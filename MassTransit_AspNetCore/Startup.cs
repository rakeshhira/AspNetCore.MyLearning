using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace MassTransit_AspNetCore
{
	public class Startup
	{
		// This method gets called by the runtime. Use this method to add services to the container.
		// For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			services.AddScoped<SendMessageConsumer>();
			services.AddMassTransit(c =>
			{
				c.AddConsumer<SendMessageConsumer>();
			});
			services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.UseExtensionsLogging(provider.GetRequiredService<ILoggerFactory>());

				var host = cfg.Host("rabbitmq", "/", h =>
				{
					h.Username("rabbitmq");
					h.Password("rabbitmq");
				});

				cfg.ReceiveEndpoint(host, "web-service-endpoint", e =>
				{
					e.PrefetchCount = 16;
					e.UseMessageRetry(x => x.Interval(2, 100));
					e.LoadFrom(provider);
					EndpointConvention.Map<SendMessageConsumer>(e.InputAddress);
				});
			}));
			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<Microsoft.Extensions.Hosting.IHostedService, BusService>();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseMvc();
			//app.Run(async (context) =>
			//{
			//	await context.Response.WriteAsync("Hello World!");
			//});
		}
	}
}
