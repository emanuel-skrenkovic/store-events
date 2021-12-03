using System.Collections.Generic;

namespace Store.Order.Application.Buyer
{
    public class CartView
    {
        public IEnumerable<CartEntry> Items { get; set; }
        
        public decimal Price { get; set; }
    }
}