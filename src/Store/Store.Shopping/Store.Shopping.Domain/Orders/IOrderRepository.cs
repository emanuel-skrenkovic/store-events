using System;
using System.Threading.Tasks;

namespace Store.Shopping.Domain.Orders;

public interface IOrderRepository
{
    Task<Order> GetOrderAsync(Guid orderId);

    Task SaveOrderAsync(Order order);
}