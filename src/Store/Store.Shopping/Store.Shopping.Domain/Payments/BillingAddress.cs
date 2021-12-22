using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Shopping.Domain.Payments;

public class BillingAddress : ValueObject<BillingAddress>
{
    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new System.NotImplementedException();
    }
}