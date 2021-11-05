using System;
using System.Threading;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore
{
    public abstract class EventStoreProjectionManager : IProjectionManager
    {
        private readonly EventStoreSubscriptionConfiguration _configuration;
        private readonly EventStoreClient _eventStore; 
        
        private readonly ICheckpointRepository _checkpointRepository;
        private readonly ISerializer _serializer;

        public EventStoreProjectionManager(
            EventStoreSubscriptionConfiguration configuration,
            EventStoreClient eventStore, 
            ICheckpointRepository checkpointRepository,
            ISerializer serializer)
        {
            _configuration        = configuration        ?? throw new ArgumentNullException(nameof(configuration));
            _eventStore           = eventStore           ?? throw new ArgumentNullException(nameof(eventStore));
            _checkpointRepository = checkpointRepository ?? throw new ArgumentNullException(nameof(checkpointRepository));
            _serializer           = serializer           ?? throw new ArgumentNullException(nameof(serializer));
        }
    
        public async Task StartAsync()
        {
            ulong checkpoint = await _checkpointRepository.GetAsync(_configuration.SubscriptionId) ?? 0;
            await SubscribeAt(checkpoint);
        }

        public Task StopAsync()
        {
            // TODO
            return Task.CompletedTask;
        }
        
        protected abstract Task ProjectEventAsync(IEvent @event);
        
        private async Task HandleEventAsync(StreamSubscription streamSubscription, ResolvedEvent resolvedEvent, CancellationToken cancellationToken)
        {
            try
            {
                IEvent @event = resolvedEvent.Deserialize(_serializer) as IEvent;
                
                if (@event == null) return;

                // TODO: transaction of some sort?
                await ProjectEventAsync(@event);
                await UpdateCheckpoint();
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
                    StartAsync().ConfigureAwait(false).GetAwaiter().GetResult();
                }
            }
            // TODO: think about the conditions here
            catch
            {
                StopAsync().ConfigureAwait(false).GetAwaiter().GetResult();
            }
        }
        
        private async Task SubscribeAt(ulong? checkpoint)
        {
            Position startPosition = checkpoint == null 
                ? Position.Start 
                : new Position(checkpoint.Value, checkpoint.Value);
            
            await _eventStore.SubscribeToAllAsync(
                start:                     startPosition,
                eventAppeared:             HandleEventAsync,
                resolveLinkTos:            false,
                subscriptionDropped:       HandleSubscriptionDropped,
                filterOptions:             _configuration.FilterOptions,
                configureOperationOptions: null, // TODO
                userCredentials:           _configuration.Credentials,
                CancellationToken.None
            );
        }

        private async Task UpdateCheckpoint()
        {
            // TODO: optimize
            ulong currentCheckpoint = await _checkpointRepository.GetAsync(_configuration.SubscriptionId) ?? 0;
            await _checkpointRepository.SaveAsync(_configuration.SubscriptionId, ++currentCheckpoint);
        }
    }
}