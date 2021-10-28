using System;
using System.Linq;
using System.Threading.Tasks;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Infrastructure.EventStore.Extensions;

namespace Store.Core.Infrastructure.EventStore
{
    public class EventStoreCheckpointRepository : ICheckpointRepository
    {
        // TODO: should this be a configuration?
        private const string CheckpointStreamNamePrefix = "projections-checkpoints-";
        
        private readonly EventStoreClient _eventStore;
        private readonly ISerializer _serializer;

        public EventStoreCheckpointRepository(EventStoreClient eventStore, ISerializer serializer)
        {
            _eventStore = eventStore ?? throw new ArgumentNullException(nameof(eventStore));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }
        
        public async Task<ulong?> GetAsync(string subscriptionId)
        {
            Guard.IsNotNullOrEmpty(subscriptionId, Guard.CommonMessages.NullOrEmpty(nameof(subscriptionId)));

            EventStoreClient.ReadStreamResult result = _eventStore.ReadStreamAsync(
                Direction.Backwards, 
                GenerateCheckpointStreamName(subscriptionId), 
                StreamPosition.End,
                maxCount: 1);
            
            ResolvedEvent finalEvent = await result.FirstOrDefaultAsync();
            return finalEvent.Deserialize(_serializer) as ulong?;
        }

        public Task SaveAsync(string subscriptionId, ulong position)
        {
            Guard.IsNotNullOrEmpty(subscriptionId, Guard.CommonMessages.NullOrEmpty(nameof(subscriptionId)));
            
            return _eventStore.AppendToStreamAsync(
                GenerateCheckpointStreamName(subscriptionId), 
                StreamState.Any, 
                new[]
                    {
                        new EventData(
                            Uuid.NewUuid(), 
                            typeof(ulong).AssemblyQualifiedName!, 
                            _serializer.SerializeToBytes(position))
                    });
        }

        private string GenerateCheckpointStreamName(string subscriptionId)
            => $"{CheckpointStreamNamePrefix}-{subscriptionId}";
    }
}