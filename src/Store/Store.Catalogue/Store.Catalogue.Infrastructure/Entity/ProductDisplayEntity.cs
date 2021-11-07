using System;
using System.Collections.Generic;
using Store.Core.Domain;
using Store.Core.Infrastructure.EntityFramework;

namespace Store.Catalogue.Infrastructure.Entity
{
    public class ProductDisplayEntity : IProjectionDocument
    {
        public Guid Id { get; set; }
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime UpdatedAt { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }
        
        public bool Available { get; set; }

        public ICollection<ProductReview> Reviews { get; set; }

        public ICollection<Tag> Tags { get; set; }

        #region Persistence
        
        public string Data { get; set; }

        public void SerializeData(ISerializer serializer)
        {
            Data = serializer.Serialize(new
            {
                Id,
                Name,
                Description,
                Price,
                Available,
                Reviews,
                Tags
            });
        }

        public void DeserializeData(ISerializer serializer)
        {
            ProductDisplayEntity deserialized = serializer.Deserialize<ProductDisplayEntity>(Data);
            
            Id = deserialized.Id;
            Name = deserialized.Name;
            Description = deserialized.Description;
            Price = deserialized.Price;
            Available = deserialized.Available;
            Reviews = deserialized.Reviews;
            Tags = deserialized.Tags;
        }

        #endregion
    }

    public class Tag
    {
        public string Value { get; set; }
    }

    public class ProductReview
    {
        public string CustomerId { get; set; }
        
        public ushort Rating { get; set; }
        
        public string Text { get; set; }
        
        public DateTime DateRated { get; set; }
    }
}