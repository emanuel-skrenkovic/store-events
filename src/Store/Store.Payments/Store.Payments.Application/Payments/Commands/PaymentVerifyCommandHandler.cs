using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;

namespace Store.Payments.Application.Payments.Commands;

public class PaymentCompleteCommandHandler : IRequestHandler<PaymentVerifyCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentCompleteCommandHandler(IPaymentRepository paymentRepository)
        => _paymentRepository = Ensure.NotNull(paymentRepository);

    public Task<Result> Handle(PaymentVerifyCommand request, CancellationToken cancellationToken)
        => _paymentRepository.GetPaymentAsync(request.PaymentId)
            .Then(async payment =>
            {
                if (payment.Status == PaymentStatus.Completed) return Result.Ok();
                
                return await payment.Verify().Then(() => _paymentRepository.SavePaymentAsync(payment));
            });
}