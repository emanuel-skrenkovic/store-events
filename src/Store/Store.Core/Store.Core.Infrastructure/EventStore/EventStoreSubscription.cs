using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore;

public class EventStoreSubscription : IEventSubscription
{
    private readonly ISerializer _serializer;
    private readonly IServiceScopeFactory _scopeFactory;
        
    private readonly EventStoreConnectionConfiguration _configuration;
    private readonly EventStoreClient _eventStore;

    private readonly Func<IEvent, EventMetadata, Task> _handleEvent;

    internal EventStoreSubscription(
        ISerializer serializer,
        IServiceScopeFactory scopeFactory,
        EventStoreConnectionConfiguration configuration,
        EventStoreClient eventStore,
        Func<IEvent, EventMetadata, Task> handleEvent)
    {
        _serializer    = Ensure.NotNull(serializer);
        _scopeFactory  = Ensure.NotNull(scopeFactory);
        _configuration = Ensure.NotNull(configuration);
        _eventStore    = Ensure.NotNull(eventStore);
        _handleEvent   = Ensure.NotNull(handleEvent);
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
                
            ulong streamPosition = resolvedEvent.OriginalPosition?.CommitPosition ?? Position.Start.CommitPosition;
            
            EventMetadata eventMetadata = _serializer.DeserializeFromBytes<EventMetadata>(resolvedEvent.Event.Metadata.ToArray());
            eventMetadata.StreamPosition = streamPosition;

            // Run the event handler in a scope, so as to have the 
            // correlation context be unique between the handlers.
            // TODO: check if this actually works this way.
            using IServiceScope scope = _scopeFactory.CreateScope();

            // TODO: think about this!
            CorrelationContext.SetMessageId(GuidUtility.NewDeterministicGuid(
                @event!.GetType().FullName, 
                eventMetadata.EventId.ToString()));
            CorrelationContext.SetCorrelationId(eventMetadata.CorrelationId);
            CorrelationContext.SetCausationId(eventMetadata.EventId);
                
            await _handleEvent(@event, eventMetadata);
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