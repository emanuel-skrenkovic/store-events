using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;
using Store.Payments.Domain.Payments.ValueObjects;

namespace Store.Payments.Application.Payments.Commands;

public class PaymentCreateCommandHandler : IRequestHandler<PaymentCreateCommand, Result<PaymentCreateResponse>>
{
    private readonly IAggregateRepository _repository;

    public PaymentCreateCommandHandler(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);
    
    public Task<Result<PaymentCreateResponse>> Handle(PaymentCreateCommand request, CancellationToken cancellationToken)
    {
        PaymentNumber paymentNumber = new(Guid.NewGuid());
        Payment payment = Payment.Create(
            paymentNumber,
            new Source(request.Source),
            new Amount(request.Amount),
            request.Note);

        return _repository.SaveAsync<Payment, Guid>(payment, CorrelationContext.CorrelationId, CorrelationContext.CausationId)
            .Then<PaymentCreateResponse>(() => new PaymentCreateResponse(paymentNumber.Value));
    }
}