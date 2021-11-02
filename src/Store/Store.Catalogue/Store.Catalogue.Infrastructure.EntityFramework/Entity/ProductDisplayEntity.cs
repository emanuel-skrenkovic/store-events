using System;
using System.Collections.Generic;
using System.Text.Json;
using Store.Core.Domain.Projection;

namespace Store.Catalogue.Infrastructure.EntityFramework.Entity
{
    public class ProductDisplayEntity : IReadModel
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }

        public ICollection<ProductReview> Reviews { get; set; }

        public ICollection<Tag> Tags { get; set; }

        #region Persistence

        private string _data;
        
        // TODO: bad
        public string Serialized
        {
            get => JsonSerializer.Serialize(new
            {
                Id,
                Name,
                Description,
                Price,
                Reviews,
                Tags
            });
            set => _data = value;
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