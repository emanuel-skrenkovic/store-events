using System;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.Extensions
{
    // TODO: think about type names
    public static class EventExtensions
    {
        private static readonly ISerializer Serializer = new JsonSerializer(); // TODO: clean this up. This is horrible. D:
        
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
                eventType.AssemblyQualifiedName, 
                Serializer.SerializeToBytes(domainEvent, eventType));
        }
    }
}