using System;
using EventStore.Client;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.Extensions
{
    // TODO: think about type names
    public static class EventExtensions
    {
        private const string TypeMetadataKey = "Type";
        
        private static readonly ISerializer _serializer = null; // TODO

        public static object Deserialize(this ResolvedEvent resolvedEvent)
        { 
            EventRecord record = resolvedEvent.Event;

            return _serializer.DeserializeFromBytes(
                record.Data.ToArray(),
                Type.GetType(record.EventType));
        }

        public static EventData ToEventData<T>(this T domainEvent) where T : IEvent
        {
            return new EventData(
                Uuid.NewUuid(), 
                domainEvent.GetType().FullName, 
                _serializer.SerializeToBytes(domainEvent));
        }
    }
}