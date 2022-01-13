using Store.Core.Domain;

namespace Store.Payments.Domain.Payments.ValueObjects;

public class OrderId : ValueObject<OrderId>
{
    public Guid Value { get; }

    public OrderId(Guid value)
    {
        if (value == default) 
            throw new ArgumentException("Product number cannot be equal to Guid default value.");
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}