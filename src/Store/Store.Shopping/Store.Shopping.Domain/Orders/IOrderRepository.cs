using System;
using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain.Orders;

public interface IOrderRepository
{
    Task<Result<Order>> GetOrderAsync(Guid orderId);

    Task<Result> SaveOrderAsync(Order order);
}