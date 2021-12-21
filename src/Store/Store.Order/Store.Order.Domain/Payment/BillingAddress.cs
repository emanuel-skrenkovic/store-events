using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.Payment;

public class BillingAddress : ValueObject<BillingAddress>
{
    protected override IEnumerable<object> GetEqualityComponents()
    {
        throw new System.NotImplementedException();
    }
}