using System;
using System.Threading.Tasks;
using Store.Core.Domain;

namespace Store.Order.Domain.Payment;

public class PaymentRepository : IPaymentRepository
{
    private readonly IAggregateRepository _repository;

    public PaymentRepository(IAggregateRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<Payment> GetPaymentAsync(Guid paymentId)
        => _repository.GetAsync<Payment, Guid>(paymentId);

    public Task SavePaymentAsync(Payment payment)
        => _repository.SaveAsync<Payment, Guid>(payment);
}