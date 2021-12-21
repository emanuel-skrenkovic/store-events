using System;
using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.Orders;

public class OrderLine : ValueObject<OrderLine>
{
    public string CatalogueNumber { get; }
        
    public decimal Price { get; }
        
    public uint Count { get; }

    public OrderLine(string catalogueNumber, decimal price, uint count)
    {
        Ensure.NotNullOrEmpty(catalogueNumber, nameof(catalogueNumber));
        if (price < 0) throw new ArgumentException($"{nameof(price)} cannot be negative.");
        if (count == 0) throw new ArgumentException($"{nameof(count)} cannot be 0.");

        CatalogueNumber = catalogueNumber;
        Price = price;
        Count = count;
    }
        
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CatalogueNumber;
        yield return Price; 
        yield return Count;
    }
}