using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;

namespace Store.Payments.Application.Payments.Commands;

public class PaymentCompleteCommandHandler : IRequestHandler<PaymentCompleteCommand, Result>
{
    private readonly IPaymentRepository _paymentRepository;

    public PaymentCompleteCommandHandler(IPaymentRepository paymentRepository)
        => _paymentRepository = Ensure.NotNull(paymentRepository);

    public Task<Result> Handle(PaymentCompleteCommand request, CancellationToken cancellationToken)
        => _paymentRepository.GetPaymentAsync(request.PaymentId)
            .Then(payment => payment.Complete().Then(() => _paymentRepository.SavePaymentAsync(payment)));
}