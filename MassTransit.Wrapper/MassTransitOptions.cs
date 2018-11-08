using Microsoft.Extensions.Configuration;

namespace MassTransit.Wrapper
{
	public class MassTransitOptions
	{
		public bool Enabled { get; set; }
		public MassTransitConnectionOptions Connection { get; set; }
		public MassTransitRetryOptions Retry { get; set; }
		public ConsumerOptions Consumer { get; set; }
	}

	public class MassTransitConnectionOptions
	{
		public string HostName { get; set; }
		public string VirtualHost { get; set; }
		public string UserName { get; set; }
		public string Password { get; set; }
	}

	public class ConsumerOptions
	{
		public ushort PrefetchCount { get; set; }
	}

	public class MassTransitRetryOptions
	{
		public int Count { get; set; }
		public int IntervalInMs { get; set; }
	}


	public static class MassTransitOptionsProvider
	{
		public static MassTransitOptions Get(IConfiguration configuration)
		{

			var massTransitOptions = configuration.GetSection("MassTransit").Get<MassTransitOptions>();

			if (massTransitOptions == null)
			{
				return null;
			}

			if (!string.IsNullOrEmpty(configuration["vcap:services:hsdp-rabbitmq:0:name"]))
			{
				massTransitOptions.Connection.HostName = configuration["vcap:services:hsdp-rabbitmq:0:credentials:hostname"];
				massTransitOptions.Connection.UserName = configuration["vcap:services:hsdp-rabbitmq:0:credentials:admin_username"];
				massTransitOptions.Connection.Password = configuration["vcap:services:hsdp-rabbitmq:0:credentials:password"];
			}

			return massTransitOptions;
		}
	}
}
