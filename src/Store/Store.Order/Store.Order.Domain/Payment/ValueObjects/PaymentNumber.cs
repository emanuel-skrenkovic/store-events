using System;
using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.Payment.ValueObjects
{
    public class PaymentNumber : ValueObject<PaymentNumber>
    {
        public Guid Value { get; }

        public PaymentNumber(Guid value)
        {
            if (value == default) throw new ArgumentException("Payment number cannot be equal to Guid default value.");
            Value = value;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}