using System;
using System.Threading;

namespace Store.Core.Domain;

// TODO: think about this.
public class CorrelationValues
{
    public Guid CorrelationId { get; set; }
    
    public Guid CausationId { get; set; }
}

public class CorrelationContext
{
    private static readonly AsyncLocal<CorrelationValues> CorrelationValues = new();
    
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
}