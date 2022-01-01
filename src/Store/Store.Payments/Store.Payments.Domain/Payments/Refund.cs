namespace Store.Payments.Domain.Payments;

public class Refund
{
    public Guid Id { get; init; }
    
    public Guid PaymentId { get; init; }
    
    public decimal Amount { get; init;  }
    
    public string Note { get; init; }
}