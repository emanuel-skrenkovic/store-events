using System;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreSubscriptionFactory : IEventSubscriptionFactory
{
    private readonly ISerializer _serializer;
    private readonly EventStoreClient _eventStore;
    private readonly EventStoreConnectionConfiguration _configuration;

    public EventStoreSubscriptionFactory(ISerializer serializer, EventStoreClient eventStore, EventStoreConnectionConfiguration configuration)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
        
    public IEventSubscription Create(string subscriptionId, Func<IEvent, EventMetadata, Task> handleEvent)
    {
        return new EventStoreSubscription(
            _serializer, 
            _configuration with { SubscriptionId = subscriptionId },
            _eventStore,
            handleEvent);
    }
}