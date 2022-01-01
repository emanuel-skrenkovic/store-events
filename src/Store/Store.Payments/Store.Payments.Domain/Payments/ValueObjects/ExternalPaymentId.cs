using Store.Core.Domain;

namespace Store.Payments.Domain.Payments.ValueObjects;

public class ExternalPaymentId : ValueObject<ExternalPaymentId>
{
    public string Value { get; }

    public ExternalPaymentId(string value)
    {
        Ensure.NotNullOrEmpty(value);
        Value = value;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}