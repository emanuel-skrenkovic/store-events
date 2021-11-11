using System;
using System.Threading.Tasks;

namespace Store.Order.Domain.Orders
{
    public interface IOrderRepository
    {
        Task<Order> GetOrderAsync(Guid orderId);

        Task CreateOrderAsync(Order order);

        Task SaveOrderAsync(Order order);
    }
}