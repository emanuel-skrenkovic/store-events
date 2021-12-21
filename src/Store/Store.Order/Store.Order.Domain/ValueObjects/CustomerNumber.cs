using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.ValueObjects;

public class CustomerNumber : ValueObject<CustomerNumber>
{
    public string Value { get; }

    public CustomerNumber(string value)
    {
        Ensure.NotNullOrEmpty(value, nameof(value));
        Value = value;
    }
        
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }

    public override string ToString() => Value;
}