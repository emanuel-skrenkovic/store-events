using System.Collections.Generic;
using System.Collections.Immutable;
using Store.Core.Domain;

namespace Store.Shopping.Domain.Orders.ValueObjects;

public class OrderLines : ValueObject<OrderLines>
{
    public IReadOnlyCollection<OrderLine> Value { get; }

    public OrderLines(ICollection<OrderLine> value)
    {
        Ensure.NotNullOrEmpty(value);
        Value = value.ToImmutableArray();
    }
        
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}