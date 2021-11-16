using Store.Core.Domain.Result;

namespace Store.Order.Domain
{
    public interface IOrderPaymentService
    {
        Result<Payment.Payment> CreateOrderPayment(Orders.Order order);
    }
}