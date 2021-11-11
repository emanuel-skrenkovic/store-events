using System;
using System.Linq;
using Store.Core.Domain;
using Store.Core.Domain.Result;
using Store.Order.Domain.Buyers;
using Store.Order.Domain.Orders;

namespace Store.Order.Domain
{
    public class CustomerOrderService : ICustomerOrderService
    {
        public Result<Orders.Order> PlaceOrder(Buyer buyer)
        {
            Ensure.NotNull(buyer, nameof(buyer));
            
            if (!buyer.Cart.Items.Any())
            {
                return new Error("No items found in cart.");
            }
            
            Orders.Order order = Orders.Order.Create(Guid.NewGuid(), buyer.Id);
            
            foreach ((Item item, uint count) in buyer.Cart.Items)
            {
                order.AddOrderLine(new OrderLine(item, count));
            }

            return order;
        }
    }
}