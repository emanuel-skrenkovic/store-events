using System;
using System.Collections.Generic;
using Store.Core.Domain;

namespace Store.Order.Domain.Orders.ValueObjects
{
    public class OrderNumber : ValueObject<OrderNumber>
    {
        public Guid Value { get; }

        public OrderNumber(Guid value)
        {
            if (value == default) throw new ArgumentException("Order number cannot be equal to Guid default value.");
            Value = value;
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}