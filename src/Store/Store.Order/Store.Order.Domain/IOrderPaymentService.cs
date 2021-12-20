using Store.Core.Domain.ErrorHandling;

namespace Store.Order.Domain
{
    public interface IOrderPaymentService
    {
        Result<Payment.Payment> CreateOrderPayment(Orders.Order order);
    }
}