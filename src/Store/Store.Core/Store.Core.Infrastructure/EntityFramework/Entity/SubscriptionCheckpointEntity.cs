using System;

namespace Store.Core.Infrastructure.EntityFramework.Entity;

public class SubscriptionCheckpointEntity
{
    public Guid Id { get; set; }
        
    public string SubscriptionId { get; set; }
        
    public ulong Position { get; set; }
}