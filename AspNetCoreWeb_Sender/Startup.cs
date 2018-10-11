using AspNetCoreWebCommon;
using DotNetCoreCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Wrapper.Abstractions;

namespace AspNetCoreWeb_Sender
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

			services.AddRouting();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		override public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			base.Configure(app, env);

			var routeBuilder = new RouteBuilder(app,
				new RouteHandler(context =>
				{
					var routeValues = context.GetRouteData().Values;
					return context.Response.WriteAsync($"Hello! Route values: {string.Join(", ", routeValues)}");
				}));

			routeBuilder
				.MapGet("{routingKey}/{message}", context =>
				{
					string routingKey = context.GetRouteValue("routingKey") as string;
					string message = context.GetRouteValue("message") as string;
					IRabbitMQClientWrapper rabbitMQClientWrapper = context.RequestServices.GetRequiredService<IRabbitMQClientWrapper>();
					rabbitMQClientWrapper.BasicPublish(routingKey, message.ToUtf8Bytes());
					return context.Response.WriteAsync($"{nameof(AspNetCoreWeb_Sender)} > Published '{routingKey}:{message}'");
				})
				.MapGet("", context =>
				{
					return context.Response.WriteAsync($"{nameof(AspNetCoreWeb_Sender)} > Hello world!!");
				});

			var routes = routeBuilder.Build();
			app.UseRouter(routes);
		}
	}
}
