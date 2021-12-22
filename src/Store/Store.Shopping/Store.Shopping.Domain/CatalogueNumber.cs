using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Shopping.Domain;

public class CatalogueNumber : ValueObject<CatalogueNumber>
{
    public string Value { get; }

    public CatalogueNumber(string value)
    {
        Ensure.NotNullOrEmpty(value, nameof(value));
        Value = value;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Value;
    }
}