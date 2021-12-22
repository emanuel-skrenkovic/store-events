using System;
using System.Threading.Tasks;

namespace Store.Shopping.Domain.Payments;

public interface IPaymentRepository
{
    Task<Payment> GetPaymentAsync(Guid paymentId);

    Task SavePaymentAsync(Payment payment);
}