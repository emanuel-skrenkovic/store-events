using System;

namespace Store.Catalogue.Infrastructure.Entity;

public class ProductEntity
{
    public Guid Id { get; set; }
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime UpdatedAt { get; set; }
        
    public string Name { get; set; }
        
    public string Description { get; set; }
        
    public decimal Price { get; set; }
        
    public bool Available { get; set; }
}