using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;

namespace Store.Shopping.Application.Orders.Commands.Confirm;

public class OrderConfirmCommandHandler : IRequestHandler<OrderConfirmCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public OrderConfirmCommandHandler(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result> Handle(OrderConfirmCommand request, CancellationToken cancellationToken) =>
        _repository.GetAsync<Order, Guid>(request.OrderId)
            .Then(order => order.Confirm()
                .Then(() => _repository.SaveAsync<Order, Guid>(order)));
}