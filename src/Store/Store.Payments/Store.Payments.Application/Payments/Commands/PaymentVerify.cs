using MediatR;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;
using Store.Payments.Domain.Payments;

namespace Store.Payments.Application.Payments.Commands;

public record PaymentVerifyCommand(Guid PaymentId) : IRequest<Result>;

public class PaymentVerify : IRequestHandler<PaymentVerifyCommand, Result>
{
    private readonly IAggregateRepository _repository;

    public PaymentVerify(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);

    public Task<Result> Handle(PaymentVerifyCommand request, CancellationToken cancellationToken)
        => _repository.GetAsync<Payment, Guid>(request.PaymentId)
            .Then(async payment =>
            {
                if (payment.Status == PaymentStatus.Verified) return Result.Ok();

                return await payment
                    .Verify()
                    .Then(() => _repository.SaveAsync<Payment, Guid>(payment));
            });
}