using Store.Order.Application.Product;

namespace Store.Order.Application.Buyer
{
    public class CartEntry
    {
        public ProductInfo Item { get; set; }
        
        public uint Quantity { get; set; }
        
        public decimal Price { get; set; }
    }
}