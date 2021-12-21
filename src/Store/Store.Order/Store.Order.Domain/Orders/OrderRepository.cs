using System;
using System.Threading.Tasks;
using Store.Core.Domain;

namespace Store.Order.Domain.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly IAggregateRepository _repository;

    public OrderRepository(IAggregateRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
        
    public Task<Order> GetOrderAsync(Guid orderId)
    {
        return _repository.GetAsync<Order, Guid>(orderId);
    }

    public Task SaveOrderAsync(Order order)
    {
        return _repository.SaveAsync<Order, Guid>(order);
    }
}