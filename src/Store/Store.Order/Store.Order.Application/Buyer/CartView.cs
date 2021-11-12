using System.Collections.Generic;

namespace Store.Order.Application.Buyer
{
    public class CartView
    {
        public Dictionary<string, CartEntry> Items { get; set; }
    }
}