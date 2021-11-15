using System;
using EventStore.Client;
using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Core.Infrastructure.EventStore.Extensions
{
    // TODO: think about type names
    public static class EventExtensions
    {
        public static object Deserialize(this ResolvedEvent resolvedEvent, ISerializer serializer)
        { 
            EventRecord record = resolvedEvent.Event;

            return serializer.DeserializeFromBytes(
                record.Data.ToArray(),
                Type.GetType(record.EventType));
        }

        public static EventData ToEventData<T>(this T domainEvent, ISerializer serializer) 
        {
            Type eventType = domainEvent.GetType();
            
            return new EventData(
                Uuid.NewUuid(), 
                eventType.AssemblyQualifiedName!,
                serializer.SerializeToBytes(domainEvent, eventType));
        }
    }
}