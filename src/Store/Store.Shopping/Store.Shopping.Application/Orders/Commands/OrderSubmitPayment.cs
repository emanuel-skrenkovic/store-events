using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Orders;
using Store.Shopping.Domain.ValueObjects;

namespace Store.Shopping.Application.Orders.Commands;

public record OrderSubmitPaymentCommand(Guid OrderId, Guid PaymentId) : IRequest<Result>;

public class OrderSubmitPayment : IRequestHandler<OrderSubmitPaymentCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public OrderSubmitPayment(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result> Handle(OrderSubmitPaymentCommand request, CancellationToken ct) 
        => _repository
            .GetAsync<Order, Guid>(request.OrderId, ct)
            .Then
            (
                order => order
                    .SubmitPayment(new PaymentNumber(request.PaymentId))
                    .Then(() => _repository.SaveAsync<Order, Guid>(order, ct))
            );
}