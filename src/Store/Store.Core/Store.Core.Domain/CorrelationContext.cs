using System;
using System.Threading;
using Store.Core.Domain.Event;

namespace Store.Core.Domain;

// TODO: think about this.
public class CorrelationValues
{
    public Guid MessageId { get; set; }
    
    public Guid CorrelationId { get; set; }
    
    public Guid CausationId { get; set; }
}

// TODO: think about using regular DI instead of AsyncLocal<T>
public class CorrelationContext
{
    private static readonly AsyncLocal<CorrelationValues> CorrelationValues = new();
    
    public static Guid MessageId => CorrelationValues.Value?.MessageId ?? default;
    
    public static Guid CorrelationId => CorrelationValues.Value?.CorrelationId ?? default;
    
    public static Guid CausationId => CorrelationValues.Value?.CausationId ?? default;

    public static void SetCorrelationId(Guid correlationId)
    {
        if (default == correlationId) 
            throw new ArgumentException(
                $"{nameof(correlationId)} cannot be equal to the default value of {correlationId.GetType()}");
        
        CorrelationValues.Value ??= new();

        if (CorrelationValues.Value.CorrelationId != default)
            throw new InvalidOperationException("Correlation id is already set.");

        CorrelationValues.Value.CorrelationId = correlationId;
    }
    
    public static void SetCausationId(Guid causationId)
    {
        if (default == causationId) 
            throw new ArgumentException(
                $"{nameof(causationId)} cannot be equal to the default value of {causationId.GetType()}");

        CorrelationValues.Value ??= new();
        
        if (CorrelationValues.Value.CausationId != default)
            throw new InvalidOperationException("Causation id is already set.");

        CorrelationValues.Value.CausationId = causationId;
    }
    
    public static void SetMessageId(Guid messageId)
    {
        if (default == messageId) 
            throw new ArgumentException(
                $"{nameof(messageId)} cannot be equal to the default value of {messageId.GetType()}");

        CorrelationValues.Value ??= new();
        
        if (CorrelationValues.Value.MessageId != default)
            throw new InvalidOperationException("Message id is already set.");

        CorrelationValues.Value.MessageId = messageId;
    }

    public static EventMetadata CreateEventMetadata(IEvent @event) => new()
    {
        EventId = GuidUtility.NewDeterministicGuid(@event.GetType().FullName, MessageId + @event.GetType().FullName),
        CorrelationId = CorrelationId, 
        CausationId = CausationId 
    };
}