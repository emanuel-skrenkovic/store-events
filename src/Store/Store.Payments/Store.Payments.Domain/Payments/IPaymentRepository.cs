using Store.Core.Domain.ErrorHandling;

namespace Store.Payments.Domain.Payments;

public interface IPaymentRepository
{
    Task<Result<Payment>> GetPaymentAsync(Guid id);
    
    Task<Result> SavePaymentAsync(Payment payment);
}