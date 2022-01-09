using Store.Core.Domain;

namespace Store.Inventory.Domain.ValueObjects;

public class ProductNumber : ValueObject<ProductNumber>
{
    public Guid Value { get; }

    public ProductNumber(Guid value)
    {
        if (value == default) throw new ArgumentException("Product number cannot be equal to Guid default value.");
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}