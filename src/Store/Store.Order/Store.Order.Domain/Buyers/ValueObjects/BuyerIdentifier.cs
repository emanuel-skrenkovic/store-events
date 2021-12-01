using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.Buyers.ValueObjects
{
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
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CustomerNumber;
            yield return SessionId;
        }

        public override string ToString() => $"{CustomerNumber}-{SessionId}"; // TODO: think about splitter char.
    }
}