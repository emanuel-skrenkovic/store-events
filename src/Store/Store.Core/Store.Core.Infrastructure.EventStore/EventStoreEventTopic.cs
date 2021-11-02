using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Event.Integration;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore
{
    public class EventStoreEventTopic : IEventTopic, IDisposable
    {
        private StreamSubscription _subscription;
        private ulong _checkpoint;
        
        private readonly EventStoreClient _eventStore;
        private readonly EventStoreEventTopicConfiguration _configuration;
        
        private readonly ICheckpointRepository _checkpointRepository;
        private readonly IEventBus _eventBus;
        private readonly ISerializer _serializer;
        
        // TODO: needs configuration 
        public EventStoreEventTopic(
            EventStoreClient                  eventStore, 
            EventStoreEventTopicConfiguration configuration,
            ICheckpointRepository checkpointRepository, 
            IEventBus                         eventBus,
            ISerializer                       serializer) 
        {
            _eventStore           = eventStore           ?? throw new ArgumentNullException(nameof(eventStore));
            _configuration        = configuration        ?? throw new ArgumentNullException(nameof(configuration));
            _checkpointRepository = checkpointRepository ?? throw new ArgumentNullException(nameof(checkpointRepository));
            _eventBus             = eventBus             ?? throw new ArgumentNullException(nameof(eventBus));
            _serializer           = serializer           ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task StartListeningAsync()
        {
            _checkpoint = await _checkpointRepository.GetAsync(_configuration.SubscriptionId) ?? 0;
            await SubscribeAt(_checkpoint);
        }

        public Task StopListeningAsync()
        {
            // TODO: seems bad, dunno.
            Dispose();
            return Task.CompletedTask;
        }

        private async Task HandleEvent(StreamSubscription subscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            try
            {
                IEvent @event = resolvedEvent.Deserialize(_serializer) as IEvent;
                
                if (@event == null) return;

                await _eventBus.PublishAsync(@event);
                await _checkpointRepository.SaveAsync(_subscription.SubscriptionId, ++_checkpoint);
            }
            catch (Exception ex)
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
                    RestartListeningAsync()
                        .GetAwaiter()
                        .GetResult();
                }
            }
            // TODO: think about the conditions here
            catch
            {
                StopListeningAsync()
                    .GetAwaiter()
                    .GetResult();
            }
        }

        private Task RestartListeningAsync()
        {
            if (_disposed) _disposed = false;
            return StartListeningAsync();
        }

        private async Task SubscribeAt(ulong? checkpoint)
        {
            Position startPosition = checkpoint == null 
                ? Position.Start 
                : new Position(checkpoint.Value, checkpoint.Value);
            
            _subscription = await _eventStore.SubscribeToAllAsync(
                start:                     startPosition,
                eventAppeared:             HandleEvent,
                resolveLinkTos:            false,
                subscriptionDropped:       HandleSubscriptionDropped,
                filterOptions:             _configuration.FilterOptions,
                configureOperationOptions: null, // TODO
                userCredentials:           _configuration.Credentials,
                CancellationToken.None
            );
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(_subscription));
            }
        }
        
        #region Dispose

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            
            if (disposing)
            {
                _subscription?.Dispose();
                _subscription = null; 
            }

            _disposed = true;
        }
        
        #endregion
    }
}