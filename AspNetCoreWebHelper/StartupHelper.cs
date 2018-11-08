using System;
using System.Reflection;
using MassTransit.Wrapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreWebCommon
{
	public static class StartupHelper
	{
		public static void ConfigureServices(IServiceCollection services, IConfiguration configuration, params Type[] consumerTypes)
		{
			services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
			services.AddSwagger(configuration);

			string entryAssemblyName = Assembly.GetEntryAssembly().GetName().Name;
			services.AddMassTransitServices(configuration, entryAssemblyName, consumerTypes);
		}

		public static void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			app.UseSwagger(configuration);
			app.UseMvc();
		}
	}
}
