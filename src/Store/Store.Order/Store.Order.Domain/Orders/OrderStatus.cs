namespace Store.Order.Domain.Orders;

// TODO: think if this is required
public enum OrderStatus
{
    Created,
    ShippingInformationAdded,
    PaymentDetailsAdded, // TODO
    PaymentCreated,
    PaymentConfirmed,
    Shipped,
    Delivered
}