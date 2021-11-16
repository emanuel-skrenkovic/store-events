namespace Store.Order.Domain
{
    public interface IOrderPaymentService
    {
        Payment.Payment CreateOrderPayment(Orders.Order order);
    }
}