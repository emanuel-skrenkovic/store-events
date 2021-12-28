using System;

namespace Store.Catalogue.AspNet.Models;

public record ProductApiModel
{
    public Guid Id { get; init;  }
    
    public DateTime CreatedAt { get; init; }
    
    public DateTime UpdatedAt { get; init; }
    
    public string Name { get; init;  }
    
    public decimal Price { get; init; }
    
    public bool Available { get; init; }
    
    public string Description { get; init; } = null;
}