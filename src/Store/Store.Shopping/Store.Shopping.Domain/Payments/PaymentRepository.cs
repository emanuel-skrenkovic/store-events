using System;
using System.Threading.Tasks;
using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain.Payments;

public class PaymentRepository : IPaymentRepository
{
    private readonly IAggregateRepository _repository;

    public PaymentRepository(IAggregateRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<Result<Payment>> GetPaymentAsync(Guid paymentId)
        => _repository.GetAsync<Payments.Payment, Guid>(paymentId);

    public Task<Result> SavePaymentAsync(Payment payment)
        => _repository.SaveAsync<Payment, Guid>(payment);
}