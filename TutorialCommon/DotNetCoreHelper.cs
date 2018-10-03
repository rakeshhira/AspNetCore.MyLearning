using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace TutorialCommon
{
	public class DotNetCoreHelper
	{
		public static void ConfigureServices(IServiceCollection serviceCollection)
		{
			serviceCollection.AddLogging(configure => configure.AddConsole())
				.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error)
				.AddSingleton<IRabbitMQConfig, RabbitMQConfig>()
				.AddSingleton<RabbitMQClientMgr>();
		}

		public static void ConfigureJsonFile(IConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
		}

		public static bool BoolParse(string value)
		{
			string valueLowerCase = value.ToLower();
			if (valueLowerCase == "0") return false;
			if (valueLowerCase == "off") return false;
			if (valueLowerCase == "no") return false;
			if (valueLowerCase == "false") return false;

			if (valueLowerCase == "1") return true;
			if (valueLowerCase == "on") return true;
			if (valueLowerCase == "yes") return true;
			if (valueLowerCase == "true") return true;

			throw new ArgumentException($"Unknown boolean value '{value}'", nameof(value));
		}
	}
}
