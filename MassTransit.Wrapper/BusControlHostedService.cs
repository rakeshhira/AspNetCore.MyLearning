using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Hosting;

namespace MassTransit.Wrapper
{
	public class BusControlHostedService : IHostedService
	{
		private readonly IBusControl _busControl;

		public BusControlHostedService(IBusControl busControl)
		{
			_busControl = busControl;
		}

		Task IHostedService.StartAsync(CancellationToken cancellationToken)
		{
			return _busControl.StartAsync(cancellationToken);
		}

		Task IHostedService.StopAsync(CancellationToken cancellationToken)
		{
			return _busControl.StopAsync(cancellationToken);
		}
	}
}
