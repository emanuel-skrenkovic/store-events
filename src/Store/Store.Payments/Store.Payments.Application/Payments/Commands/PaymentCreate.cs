using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;
using Store.Payments.Domain.Payments.ValueObjects;

namespace Store.Payments.Application.Payments.Commands;

public record PaymentCreateCommand
(
    Guid OrderId, 
    string Source, 
    decimal Amount, 
    string Note = null
) : IRequest<Result<PaymentCreateResponse>>;

public record PaymentCreateResponse(Guid PaymentId);

public class PaymentCreate : IRequestHandler<PaymentCreateCommand, Result<PaymentCreateResponse>>
{
    private readonly IAggregateRepository _repository;

    public PaymentCreate(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result<PaymentCreateResponse>> Handle(PaymentCreateCommand request, CancellationToken ct)
    {
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        Payment payment = Payment.Create
        (
            paymentNumber,
            new OrderId(request.OrderId),
            new Source(request.Source),
            new Amount(request.Amount),
            request.Note
        );

        return _repository.SaveAsync<Payment, Guid>(payment, ct)
            .Then<PaymentCreateResponse>(() => new PaymentCreateResponse(paymentNumber.Value));
    }
}