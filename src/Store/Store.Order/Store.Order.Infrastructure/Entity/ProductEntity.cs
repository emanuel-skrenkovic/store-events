namespace Store.Order.Infrastructure.Entity;

public class ProductEntity
{
    public string CatalogueNumber { get; set; }
        
    public DateTime CreatedAt { get; set; }
        
    public DateTime UpdatedAt { get; set; }
        
    public string Name { get; set; }
        
    public decimal Price { get; set; }
        
    public bool Available { get; set; }
}