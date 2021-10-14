using System;
using EventStore.Client;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.Extensions
{
    // TODO: think about type names
    public static class EventExtensions
    {
        private const string TypeMetadataKey = "Type";
        
        private static readonly ISerializer Serializer = new JsonSerializer(); // TODO
        
        public static object Deserialize(this ResolvedEvent resolvedEvent)
        { 
            EventRecord record = resolvedEvent.Event;

            return Serializer.DeserializeFromBytes(
                record.Data.ToArray(),
                Type.GetType(record.EventType));
        }

        public static EventData ToEventData<T>(this T domainEvent) where T : IEvent
        {
            Type eventType = domainEvent.GetType();
            
            return new EventData(
                Uuid.NewUuid(), 
                eventType.FullName, 
                Serializer.SerializeToBytes(domainEvent, eventType));
        }
    }
}