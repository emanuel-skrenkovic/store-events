using Store.Core.Domain;
using Store.Core.Domain.ErrorHandling;

namespace Store.Payments.Domain.Payments;

public class PaymentRepository : IPaymentRepository
{
    private readonly IAggregateRepository _repository;

    public PaymentRepository(IAggregateRepository repository)
        => _repository = Ensure.NotNull(repository);

    public Task<Result<Payment>> GetPaymentAsync(Guid id) 
        => _repository.GetAsync<Payment, Guid>(id);

    public Task<Result> SavePaymentAsync(Payment payment)
        => _repository.SaveAsync<Payment, Guid>(payment);
}