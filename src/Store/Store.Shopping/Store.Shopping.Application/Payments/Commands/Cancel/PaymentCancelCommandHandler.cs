using MediatR;
using Store.Core.Domain.ErrorHandling;
using Store.Shopping.Domain.Payments;

namespace Store.Shopping.Application.Payments.Commands.Cancel;

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

        Result<Payment> getPaymentResult = await _paymentRepository.GetPaymentAsync(request.PaymentId);

        return await getPaymentResult.Then(payment => payment
            .Cancel()
            .Then(() => _paymentRepository.SavePaymentAsync(payment)));
    }
}