using System;
using System.Collections.Generic;
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
            
            IDictionary<string, string> metadata = 
                _serializer.DeserializeFromBytes<Dictionary<string, string>>(record.Metadata.ToArray());

            return _serializer.DeserializeFromBytes(
                record.Data.ToArray(),
                Type.GetType(record.EventType));
        }

        public static EventData ToEventData(this IEvent domainEvent)
        {
            return new EventData(
                Uuid.NewUuid(), 
                domainEvent.GetType().FullName, 
                _serializer.SerializeToBytes(domainEvent));
        }
    }
}