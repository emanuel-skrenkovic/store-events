using System;
using System.Threading.Tasks;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain.Orders;

public class OrderRepository : IOrderRepository
{
    private readonly IAggregateRepository _repository;

    public OrderRepository(IAggregateRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
        
    public Task<Result<Order>> GetOrderAsync(Guid orderId)
        =>_repository.GetAsync<Order, Guid>(orderId);

    public Task<Result> SaveOrderAsync(Order order)
        => _repository.SaveAsync<Order, Guid>(order);
}