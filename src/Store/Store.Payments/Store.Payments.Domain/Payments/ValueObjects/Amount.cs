using Store.Core.Domain;

namespace Store.Payments.Domain.Payments.ValueObjects;

public class Amount : ValueObject<Amount>
{
    public decimal Value { get; }

    public Amount(decimal value)
    {
        Value = Ensure.NonNegative(value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}