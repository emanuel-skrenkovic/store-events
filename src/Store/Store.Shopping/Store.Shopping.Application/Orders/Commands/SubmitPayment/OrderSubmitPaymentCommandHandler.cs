using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.ValueObjects;

namespace Store.Shopping.Application.Orders.Commands.SubmitPayment;

public class OrderSubmitPaymentCommandHandler : IRequestHandler<OrderSubmitPaymentCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public OrderSubmitPaymentCommandHandler(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result> Handle(OrderSubmitPaymentCommand request, CancellationToken cancellationToken) =>
        _repository.GetAsync<Order, Guid>(request.OrderId)
            .Then(order => order.SubmitPayment(new PaymentNumber(request.PaymentId))
                .Then(() => _repository.SaveAsync<Order, Guid>(order)));
}