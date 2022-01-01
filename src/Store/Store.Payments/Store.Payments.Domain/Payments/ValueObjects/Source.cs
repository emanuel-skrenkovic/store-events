using Store.Core.Domain;

namespace Store.Payments.Domain.Payments.ValueObjects;

public class Source : ValueObject<Source>
{
    public string Value { get; }

    public Source(string value)
    {
        Value = Ensure.NotNullOrEmpty(value);
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}