using Store.Core.Domain.Result;
using Store.Order.Domain.Buyers;

namespace Store.Order.Domain
{
    public interface ICustomerOrderService
    {
        Result<Orders.Order> PlaceOrder(Buyer buyer);
    }
}