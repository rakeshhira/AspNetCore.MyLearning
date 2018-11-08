using AspNetCoreWebCommon;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Task = System.Threading.Tasks.Task;

namespace AspNetCoreWebApp3
{
	public class Startup
	{
		private readonly IConfiguration _configuration;

		public Startup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			StartupHelper.ConfigureServices(services, _configuration, typeof(JObjectConsumer));
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			StartupHelper.Configure(app, env, _configuration);
		}
	}

	public class JObjectConsumer : IConsumer<JObject>
	{
		Task IConsumer<JObject>.Consume(ConsumeContext<JObject> context)
		{
			return Task.CompletedTask;
		}
	}
}
