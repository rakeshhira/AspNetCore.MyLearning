using System.Threading.Tasks;
using AspNetCoreWebCommon;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;

namespace AspNetCoreWebApp1
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
			StartupHelper.ConfigureServices(services, _configuration, typeof(JObjectFaultConsumer));
		}

		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			StartupHelper.Configure(app, env, _configuration);
		}
	}

	public class JObjectFaultConsumer : IConsumer<Fault<JObject>>
	{
		Task IConsumer<Fault<JObject>>.Consume(ConsumeContext<Fault<JObject>> context)
		{
			return Task.CompletedTask;
		}
	}
}
