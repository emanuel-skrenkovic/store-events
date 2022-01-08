using System;

namespace Store.Core.Domain.Event;

public class EventMetadata
{
    public ulong StreamPosition { get; set; }
    
    public Guid EventId { get; set; }
    
    public Guid CorrelationId { get; set; }
    
    public Guid CausationId { get; set; }
}
