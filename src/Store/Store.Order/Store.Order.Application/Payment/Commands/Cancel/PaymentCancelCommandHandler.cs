using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Order.Domain.Payment;

namespace Store.Order.Application.Payment.Commands.Cancel;

public class PaymentCancelCommandHandler : IRequestHandler<PaymentCancelCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentCancelCommandHandler(IPaymentRepository paymentRepository)
    {
        _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
    } 
        
    public async Task<Result> Handle(PaymentCancelCommand request, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        Domain.Payment.Payment payment = await _paymentRepository.GetPaymentAsync(request.PaymentId);
        if (payment == null) return new NotFoundError($"Payment with payment number: {request.PaymentId} could not be found.");

        payment.Cancel();
        await _paymentRepository.SavePaymentAsync(payment);

        return Result.Ok();
    }
}