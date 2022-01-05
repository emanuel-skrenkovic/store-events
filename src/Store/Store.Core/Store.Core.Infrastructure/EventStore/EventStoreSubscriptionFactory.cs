using System;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreSubscriptionFactory : IEventSubscriptionFactory
{
    private readonly ISerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly EventStoreClient _eventStore;
    private readonly EventStoreConnectionConfiguration _configuration;

    public EventStoreSubscriptionFactory(
        ISerializer serializer, 
        IServiceScopeFactory scopeFactory,
        EventStoreClient eventStore, 
        EventStoreConnectionConfiguration configuration)
    {
        _serializer    = Ensure.NotNull(serializer);
        _scopeFactory  = Ensure.NotNull(scopeFactory);
        _eventStore    = Ensure.NotNull(eventStore);
        _configuration = Ensure.NotNull(configuration);
    }
        
    public IEventSubscription Create(string subscriptionId, Func<IEvent, EventMetadata, Task> handleEvent)
    {
        return new EventStoreSubscription(
            _serializer,
            _scopeFactory,
            _configuration with { SubscriptionId = subscriptionId },
            _eventStore,
            handleEvent);
    }
}