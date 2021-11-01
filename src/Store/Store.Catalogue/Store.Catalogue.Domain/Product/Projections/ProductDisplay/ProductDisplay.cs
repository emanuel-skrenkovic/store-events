using System;
using System.Collections.Generic;

namespace Store.Catalogue.Domain.Product.Projections.ProductDisplay
{
    public class ProductDisplay
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public decimal Price { get; set; }

        public ICollection<ProductReview> Reviews { get; set; }

        public ICollection<Tag> Tags { get; set; }
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