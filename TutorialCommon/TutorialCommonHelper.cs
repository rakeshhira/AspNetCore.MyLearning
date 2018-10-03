using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TutorialCommon
{
	public class TutorialCommonHelper
	{
		public static RabbitMQClientMgr Initialize()
		{
			return Initialize(new List<string>());
		}

		public static RabbitMQClientMgr Initialize(List<string> routingKeys)
		{
			var builder = new ConfigurationBuilder();
			DotNetCoreHelper.ConfigureJsonFile(builder);
			IConfigurationRoot configurationRoot = builder.Build();

			IServiceCollection serviceCollection = new ServiceCollection();
			DotNetCoreHelper.ConfigureServices(serviceCollection);
			ServiceProvider serviceProvider = serviceCollection.BuildServiceProvider();

			IRabbitMQConfig rabbitMQConfig = serviceProvider.GetService<IRabbitMQConfig>();
			IConfigurationSection rabbitMQSection = configurationRoot.GetSection(RabbitMQConfigNames.Section);
			rabbitMQConfig.Initialize(rabbitMQSection);

			if (routingKeys.Count > 0)
			{
				rabbitMQConfig.QueueBinding.RoutingKeys.Clear();
				foreach (var severity in routingKeys)
				{
					rabbitMQConfig.QueueBinding.RoutingKeys.Add(severity);
				}
			}

			RabbitMQClientMgr rabbitMQClientMgr = serviceProvider.GetService<RabbitMQClientMgr>();
			rabbitMQClientMgr.Initialize();

			return rabbitMQClientMgr;
		}
	}
}
