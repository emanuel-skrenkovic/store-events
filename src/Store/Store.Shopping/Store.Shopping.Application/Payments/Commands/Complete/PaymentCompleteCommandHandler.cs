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

        Payment payment = await _paymentRepository.GetPaymentAsync(request.PaymentId);
        if (payment == null) return new NotFoundError($"Payment with payment number: {request.PaymentId} could not be found.");

        payment.Complete();
        await _paymentRepository.SavePaymentAsync(payment);

        return Result.Ok();
    }
}