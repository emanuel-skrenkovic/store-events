using System;
using Store.Core.Domain;
using Store.Core.Infrastructure.EntityFramework;

namespace Store.Order.Infrastructure.Entity
{
    public class ProductInfoEntity : IProjectionDocument
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string Name { get; set; }
        
        public decimal Price { get; set; }
        
        public bool Available { get; set; }
        
        public string Data { get; set; }
        public void SerializeData(ISerializer serializer)
        {
            Data = serializer.Serialize(new
            {
                Id,
                Name,
                Price,
                Available
            });
        }

        public void DeserializeData(ISerializer serializer)
        {
            ProductInfoEntity deserialized = serializer.Deserialize<ProductInfoEntity>(Data);
            
            Id = deserialized.Id;
            Name = deserialized.Name;
            Price = deserialized.Price;
            Available = deserialized.Available;
        }
    }
}