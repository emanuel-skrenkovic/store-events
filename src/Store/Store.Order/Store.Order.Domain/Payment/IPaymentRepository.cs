using System;
using System.Threading.Tasks;

namespace Store.Order.Domain.Payment;

public interface IPaymentRepository
{
    Task<Payment> GetPaymentAsync(Guid paymentId);

    Task SavePaymentAsync(Payment payment);
}