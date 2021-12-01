namespace Store.Order.Domain.Payment
{
    public enum PaymentStatus
    {
        Approved, 
        Pending, 
        Completed, 
        Cancelled, 
        Failed 
    }
}