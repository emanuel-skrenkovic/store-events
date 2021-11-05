using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure
{
    public class EventStoreSubscriptionService : IHostedService
    {
        private readonly IEnumerable<IProjectionManager> _projectionManagers;
        
        public EventStoreSubscriptionService(IEnumerable<IProjectionManager> projectionManagers)
        {
            _projectionManagers = projectionManagers ?? throw new ArgumentNullException(nameof(projectionManagers));
        }

        public Task StartAsync(CancellationToken cancellationToken)
            => Task.WhenAll(_projectionManagers.Select(pm => pm.StartAsync()));

        public Task StopAsync(CancellationToken cancellationToken)
            => Task.WhenAll(_projectionManagers.Select(pm => pm.StopAsync()));
    }
}