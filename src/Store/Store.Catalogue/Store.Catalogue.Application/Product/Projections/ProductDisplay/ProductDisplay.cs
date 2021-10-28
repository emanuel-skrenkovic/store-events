using System;
using System.Collections.Generic;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public record ProductDisplay
    {
        public Guid Id { get; init; }
        
        public string Name { get; init; }
        
        public string Description { get; init; }
        
        public decimal Price { get; init; }

        public ICollection<ProductReview> Reviews { get; init; }

        public ICollection<Tag> Tags { get; init; }
    }

    public record Tag
    {
        public string Value { get; init; }
    }

    public record ProductReview
    {
        public string CustomerId { get; init; }
        
        public ushort Rating { get; init; }
        
        public string Text { get; init; }
        
        public DateTime DateRated { get; init; }
    }
}