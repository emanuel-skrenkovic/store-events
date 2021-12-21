using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreSubscription : IEventSubscription
{
    private readonly ISerializer _serializer;
        
    private readonly EventStoreConnectionConfiguration _configuration;
    private readonly EventStoreClient _eventStore;

    private readonly Func<IEvent, EventMetadata, Task> _handleEvent;

    internal EventStoreSubscription(
        ISerializer serializer,
        EventStoreConnectionConfiguration configuration,
        EventStoreClient eventStore,
        Func<IEvent, EventMetadata, Task> handleEvent)
    {
        _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
        _handleEvent = handleEvent ?? throw new ArgumentNullException(nameof(handleEvent));
    }
        
    public Task SubscribeAtAsync(ulong checkpoint)
    {
        return _eventStore.SubscribeToAllAsync(
            start: new Position(checkpoint, checkpoint),
            eventAppeared: HandleEventAsync,
            resolveLinkTos: false,
            subscriptionDropped: HandleSubscriptionDropped,
            filterOptions: _configuration.FilterOptions,
            configureOperationOptions: null, // TODO
            userCredentials: _configuration.Credentials,
            CancellationToken.None);
    }
        
    private async Task HandleEventAsync(StreamSubscription streamSubscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
    {
        try
        {
            IEvent @event = resolvedEvent.Deserialize(_serializer) as IEvent;
                
            // if (!Handles(@event)) return;

            // TODO: transaction of some sort?
            await _handleEvent(
                @event, 
                new EventMetadata(resolvedEvent.OriginalPosition?.CommitPosition ?? Position.Start.CommitPosition));
        }
        catch
        {
            // TODO: logging
        }
    }
        
    private void HandleSubscriptionDropped(StreamSubscription subscription, SubscriptionDroppedReason reason, Exception ex)
    {
        // TODO: logging
        try
        {
            if (reason != SubscriptionDroppedReason.Disposed) {
                // TODO: should I wait here?
                // Resubscribe if the client didn't stop the subscription
                // StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                    
                // TODO: ENTIRE IMPLEMENTATION!
            }
        }
        // TODO: think about the conditions here
        catch
        {
            // StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}