using Store.Core.Domain.ErrorHandling;

namespace Store.Shopping.Domain;

public interface IOrderPaymentService
{
    Result<Payments.Payment> CreateOrderPayment(Orders.Order order);
}