namespace Store.Shopping.Infrastructure.Entity;

public class CartEntry
{
    public ProductInfo ProductInfo { get; set; }
    
    public decimal Price { get; set; }
    
    public uint Quantity { get; set; }
}