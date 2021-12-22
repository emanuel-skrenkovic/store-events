namespace Store.Shopping.Domain.Payments;

public enum PaymentStatus
{
    Approved, 
    Pending, 
    Completed, 
    Cancelled, 
    Failed 
}