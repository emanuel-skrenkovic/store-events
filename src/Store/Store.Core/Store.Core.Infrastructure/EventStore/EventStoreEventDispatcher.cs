using System;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreEventDispatcher : IEventDispatcher
{
    private readonly ISerializer _serializer;
    private readonly EventStoreClient _eventStore;
    private readonly EventStoreEventDispatcherConfiguration _configuration;
       
    public EventStoreEventDispatcher(
        ISerializer serializer, 
        EventStoreClient eventStore,
        EventStoreEventDispatcherConfiguration configuration)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }
        
    public Task DispatchAsync(object @event)
    {
        Ensure.NotNull(@event);

        return _eventStore.AppendToStreamAsync(
            _configuration.IntegrationStreamName,
            StreamState.Any, // TODO: fix this
            new [] 
            { 
                @event.ToEventData(CorrelationContext.CreateEventMetadata(@event as IEvent), _serializer) 
            });
    }
}