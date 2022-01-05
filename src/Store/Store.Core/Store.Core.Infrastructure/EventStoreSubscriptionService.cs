using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure;

public class EventStoreSubscriptionService : IHostedService
{
    private readonly IEnumerable<IEventListener> _eventListeners;
        
    public EventStoreSubscriptionService(IEnumerable<IEventListener> projectionManagers)
    {
        _eventListeners = projectionManagers ?? throw new ArgumentNullException(nameof(projectionManagers));
    }

    public Task StartAsync(CancellationToken cancellationToken)
        => Task.WhenAll(_eventListeners.Select(pm => pm.StartAsync()));

    public Task StopAsync(CancellationToken cancellationToken)
        => Task.WhenAll(_eventListeners.Select(pm => pm.StopAsync()));
}