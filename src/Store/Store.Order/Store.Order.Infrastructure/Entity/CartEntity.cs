using System;
using System.Collections.Generic;
using Store.Core.Domain;
using Store.Core.Infrastructure.EntityFramework;

namespace Store.Order.Infrastructure.Entity
{
    public class CartEntity : IProjectionDocument
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string CustomerNumber { get; set; }
        
        public Dictionary<string, CartEntryEntity> Items { get; set; }
        
        public string Data { get; set; }
        public void SerializeData(ISerializer serializer)
        {
            Data = serializer.Serialize(new
            {
                Id,  
                Items
            });
        }

        public void DeserializeData(ISerializer serializer)
        {
            CartEntity deserialized = serializer.Deserialize<CartEntity>(Data);

            Id = deserialized.Id;
            Items = deserialized.Items;
        }
    }
}