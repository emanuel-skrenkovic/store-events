using System;
using System.Text;

namespace Store.Order.Domain.Payment
{
    public class Base64PaymentNumberGenerator : IPaymentNumberGenerator
    {
        public string Generate()
        {
            Guid uniqueIdentifier = Guid.NewGuid();
            byte[] identifierBytes = Encoding.UTF8.GetBytes(uniqueIdentifier.ToString());

            return Convert.ToBase64String(identifierBytes.AsSpan());
        }
    }
}