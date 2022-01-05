namespace Store.Checkout.Application;

public enum CheckoutStatus
{
    OrderPlaced,
    PaymentVerified,
    OrderReceivedByWarehouse,
    Shipped,
    Received
}

public record CheckoutState(CheckoutStatus Status);