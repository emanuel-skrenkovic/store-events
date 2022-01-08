using System;

namespace Store.Core.Infrastructure;

public class ProcessEntity
{
    public int Id { get; set; }
    
    public Guid CorrelationId { get; set; }
    
    public string Type { get; set; }
    
    public string Data { get; set; }
}