using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.Buyers.ValueObjects;

public class BuyerIdentifier : ValueObject<BuyerIdentifier>
{
    public string CustomerNumber { get; }
        
    public string SessionId { get; }

    public BuyerIdentifier(string customerNumber, string sessionId)
    {
        Ensure.NotNullOrEmpty(customerNumber, nameof(customerNumber));
        Ensure.NotNullOrEmpty(sessionId, nameof(sessionId));

        CustomerNumber = customerNumber;
        SessionId = sessionId;
    }

    public static BuyerIdentifier FromString(string buyerId)
    {
        Ensure.NotNullOrEmpty(buyerId, nameof(buyerId));
        string[] parts = buyerId.Split(':');
        return new(parts[0], parts[1]);
    }
        
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return CustomerNumber;
        yield return SessionId;
    }

    public override string ToString() => $"{CustomerNumber}:{SessionId}"; // TODO: think about splitter char.
}