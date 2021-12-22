namespace Store.Shopping.Infrastructure.Entity;

public class Order
{
    public string CustomerNumber { get; set; }
    
    public string CustomerEmail { get; set; }
    
    public decimal TotalAmount { get; set; }
        
    public ICollection<OrderLine> OrderLines { get; set; }
        
    public ShippingInformation ShippingInformation { get; set; }
}