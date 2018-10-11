using AspNetCoreWebCommon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AspNetCoreWebApp1
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
			// add code here
		}

		override public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			base.Configure(app, env);
			// add code here
		}
	}
}
