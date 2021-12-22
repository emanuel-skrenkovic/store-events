using System;
using System.Threading.Tasks;
using Store.Core.Domain;

namespace Store.Shopping.Domain.Payments;

public class PaymentRepository : IPaymentRepository
{
    private readonly IAggregateRepository _repository;

    public PaymentRepository(IAggregateRepository repository)
        => _repository = repository ?? throw new ArgumentNullException(nameof(repository));

    public Task<Payments.Payment> GetPaymentAsync(Guid paymentId)
        => _repository.GetAsync<Payments.Payment, Guid>(paymentId);

    public Task SavePaymentAsync(Payments.Payment payment)
        => _repository.SaveAsync<Payments.Payment, Guid>(payment);
}