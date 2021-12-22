namespace Store.Shopping.Infrastructure.Entity;

public class OrderLine
{
    public string ProductCatalogueNumber { get; set; }
        
    public ProductEntity Product { get; set; }
        
    public uint Count { get; set; }
        
    public decimal TotalAmount { get; set; }
}