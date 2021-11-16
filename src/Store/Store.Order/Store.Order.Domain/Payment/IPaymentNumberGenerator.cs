namespace Store.Order.Domain.Payment
{
    public interface IPaymentNumberGenerator
    {
        string Generate();
    }
}