using System;
using System.Linq;
using GreenPipes;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MassTransit.Wrapper
{
	public static class MassTransitServiceCollectionExtensions
	{
		private static object InvokeMethod(
			object obj,
			string methodName,
			Type[] genericArgumentTypes,
			object[] parameters,
			Type classType,
			Type extensionClassType)
		{
			bool isStatic = obj == null;
			bool isExtensionMethod = extensionClassType != null;
			bool isGenericMethod = genericArgumentTypes != null && genericArgumentTypes.Length > 0;
			int genericArgumentCount = genericArgumentTypes?.Length ?? 0;
			int parameterCount = parameters?.Length ?? 0;
			parameterCount += isExtensionMethod ? 1 : 0;

			var type = isExtensionMethod ? extensionClassType : classType;
			var methodInfo = type
				.GetMethods()
				.Where(x =>
					x.Name == methodName
					&& x.IsGenericMethod == isGenericMethod
					&& x.GetGenericArguments().Length == genericArgumentCount
					&& x.GetParameters().Length == parameterCount)
				.First();

			if (isGenericMethod)
			{
				methodInfo = methodInfo.MakeGenericMethod(genericArgumentTypes);
			}

			if (isExtensionMethod)
			{
				if (parameters != null)
				{
					return methodInfo.Invoke(null, new object[] { obj }.Union(parameters).ToArray());
				}

				return methodInfo.Invoke(null, new object[] { obj });
			}

			if (isStatic) return methodInfo.Invoke(null, parameters);
			return methodInfo.Invoke(obj, parameters);
		}

		private static object InvokeInstanceMethod(
			object obj,
			string methodName,
			Type[] genericArgumentTypes,
			object[] parameters)
		{
			return InvokeMethod(obj, methodName, genericArgumentTypes, parameters, obj.GetType(), null);
		}

		private static object InvokeInterfaceMethod(
			object obj,
			string methodName,
			Type[] genericArgumentTypes,
			object[] parameters,
			Type interfaceType)
		{
			return InvokeMethod(obj, methodName, genericArgumentTypes, parameters, interfaceType, null);
		}

		private static object InvokeStaticMethod(
			Type type,
			string methodName,
			Type[] genericArgumentTypes,
			object[] parameters)
		{
			return InvokeMethod(null, methodName, genericArgumentTypes, parameters, type, null);
		}

		private static object InvokeExtensionMethod(
			object obj,
			string methodName,
			Type[] genericArgumentTypes,
			object[] parameters,
			Type extensionClassType)
		{
			return InvokeMethod(obj, methodName, genericArgumentTypes, parameters, null, extensionClassType);
		}

		private static IServiceCollection InvokeServiceCollectionAddScoped(IServiceCollection services, Type[] scopedTypes)
		{
			if (scopedTypes == null) return services;

			foreach (Type scopedType in scopedTypes)
			{
				services = (IServiceCollection)InvokeExtensionMethod(
					obj: services,
					methodName: "AddScoped",
					genericArgumentTypes: new Type[] { scopedType },
					parameters: null,
					extensionClassType: typeof(ServiceCollectionServiceExtensions));
			}
			return services;
		}

		private static void InvokeServiceCollectionConfiguratorAddConsumer(IServiceCollectionConfigurator configurator, Type[] consumerTypes)
		{
			if (consumerTypes == null) return;

			foreach (Type consumerType in consumerTypes)
			{
				InvokeInterfaceMethod(
					obj: configurator,
					methodName: "AddConsumer",
					genericArgumentTypes: new Type[] { consumerType },
					parameters: null,
					interfaceType: typeof(IServiceCollectionConfigurator));
			}
		}

		private static void InvokeEndpointConventionMap(Type[] consumerTypes, Uri destinationAddress)
		{
			if (consumerTypes == null) return;

			foreach (Type consumerType in consumerTypes)
			{
				InvokeStaticMethod(
					type: typeof(EndpointConvention),
					methodName: "Map",
					genericArgumentTypes: new Type[] { consumerType },
					parameters: new object[] { destinationAddress });
			}
		}

		public static IServiceCollection AddMassTransitServices(this IServiceCollection services, IConfiguration configuration, string apiName, params Type[] consumerTypes)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			var massTransitOptions = MassTransitOptionsProvider.Get(configuration);
			if (massTransitOptions == null)
			{
				return services;
			}

			InvokeServiceCollectionAddScoped(services, consumerTypes);
			services.AddMassTransit(cfg =>
			{
				InvokeServiceCollectionConfiguratorAddConsumer(cfg, consumerTypes);
			});

			services.AddSingleton(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
			{
				cfg.UseExtensionsLogging(provider.GetRequiredService<ILoggerFactory>());

				var host = cfg.Host(massTransitOptions.Connection.HostName, massTransitOptions.Connection.VirtualHost, h =>
				{
					h.Username(massTransitOptions.Connection.UserName);
					h.Password(massTransitOptions.Connection.Password);
				});

				cfg.ReceiveEndpoint(host, apiName, e =>
				{
					e.PrefetchCount = massTransitOptions.Consumer.PrefetchCount;
					e.UseMessageRetry(x => x.Interval(massTransitOptions.Retry.Count, TimeSpan.FromMilliseconds(massTransitOptions.Retry.IntervalInMs)));
					e.LoadFrom(provider);
					InvokeEndpointConventionMap(consumerTypes, e.InputAddress);
				});
			}));

			services.AddSingleton<IBus>(provider => provider.GetRequiredService<IBusControl>());
			services.AddSingleton<IHostedService, BusControlHostedService>();
			return services;
		}

		public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration, string apiName)
		{
			return AddMassTransitServices(services, configuration, apiName);
		}

		public static IServiceCollection AddMassTransit<TConsumer>(this IServiceCollection services, IConfiguration configuration, string apiName)
			where TConsumer : class, IConsumer
		{
			return AddMassTransitServices(services, configuration, apiName, typeof(TConsumer));
		}

		public static IServiceCollection AddMassTransit<TConsumer1, TConsumer2>(this IServiceCollection services, IConfiguration configuration, string apiName)
			where TConsumer1 : class, IConsumer
			where TConsumer2 : class, IConsumer
		{
			return AddMassTransitServices(services, configuration, apiName, typeof(TConsumer1), typeof(TConsumer2));
		}
	}
}
