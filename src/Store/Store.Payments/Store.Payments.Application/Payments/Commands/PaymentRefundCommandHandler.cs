using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;

namespace Store.Payments.Application.Payments.Commands;

public class PaymentRefundCommandHandler : IRequestHandler<PaymentRefundCommand, Result<PaymentRefundResponse>>
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentRefundCommandHandler(IPaymentRepository paymentRepository)
        => _paymentRepository = Ensure.NotNull(paymentRepository);
    
    // TODO: paying beer for anyone who can read this shit.
    public Task<Result<PaymentRefundResponse>> Handle(PaymentRefundCommand request, CancellationToken cancellationToken)
        => _paymentRepository.GetPaymentAsync(request.PaymentId)
            .Then(async payment =>
            {
                if (payment.Status == PaymentStatus.Refunded) return new PaymentRefundResponse(payment.RefundInfo.Id);
                
                return await payment.Refund(request.Note)
                    .Then(refund => _paymentRepository.SavePaymentAsync(payment)
                        .Then<PaymentRefundResponse>(() => new PaymentRefundResponse(refund.Id)));
            });
}