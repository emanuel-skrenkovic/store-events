using System;

namespace Store.Core.Domain.Outbox;

public class OutboxMessage
{
    public string CorrelationId { get; private set; }
    
    public string Data { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    
    public DateTime? ProcessedAt { get; private set; }

    public void Processed() => ProcessedAt = DateTime.UtcNow;

    public static OutboxMessage Create<T>(ISerializer serializer, string correlationId, T message)
    {
        Ensure.NotNull(message);

        return new()
        {
            CorrelationId = Ensure.NotNullOrEmpty(correlationId),
            Data = serializer.Serialize(message),
            CreatedAt = DateTime.UtcNow
        };
    }
}