using System;
using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain.Payments;

public interface IPaymentRepository
{
    Task<Result<Payment>> GetPaymentAsync(Guid paymentId);

    Task<Result> SavePaymentAsync(Payment payment);
}