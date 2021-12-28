using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Payments;

namespace Store.Shopping.Application.Payments.Commands.Complete;

public class PaymentCompleteCommandHandler : IRequestHandler<PaymentCompleteCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentCompleteCommandHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    }

    public async Task<Result> Handle(PaymentCompleteCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Result<Payment> getPaymentResult = await _paymentRepository.GetPaymentAsync(request.PaymentId);

        return await getPaymentResult.Then(payment => payment
            .Complete()
            .Then(() => _paymentRepository.SavePaymentAsync(payment)));
    }
}