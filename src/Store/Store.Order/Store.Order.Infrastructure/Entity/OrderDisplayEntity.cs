using System;
using System.Collections.Generic;
using Store.Core.Domain;
using Store.Core.Infrastructure.EntityFramework;

namespace Store.Order.Infrastructure.Entity
{
    public class OrderDisplayEntity : IProjectionDocument
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string CustomerNumber { get; set; }
        
        public ICollection<OrderLineEntity> OrderLines { get; set; }
        
        public ShippingInformationEntity ShippingInformation { get; set; }
        
        public string Data { get; set; }
        
        public void DeserializeData(ISerializer serializer)
        {
            OrderDisplayEntity deserialized = serializer.Deserialize<OrderDisplayEntity>(Data);

            Id = deserialized.Id;
            CustomerNumber = deserialized.CustomerNumber;
            OrderLines = deserialized.OrderLines;
            ShippingInformation = deserialized.ShippingInformation;
        }

        public void SerializeData(ISerializer serializer)
        {
            Data = serializer.Serialize(new
            {
                Id,
                CustomerNumber,
                OrderLines,
                ShippingInformation
            });
        }
    } 
}
