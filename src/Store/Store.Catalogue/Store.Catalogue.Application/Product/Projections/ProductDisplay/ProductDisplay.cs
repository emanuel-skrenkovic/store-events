using System;
using System.Collections.Generic;

namespace Store.Catalogue.Application.Product.Projections.ProductDisplay
{
    public record ProductDisplay(
        Guid Id, 
        string Name, 
        string Description, 
        decimal Price,
        ICollection<ProductReview> Reviews = null,
        ICollection<Tag> Tags = null);

    public record Tag(string Value);

    public record ProductReview(string CustomerId, ushort Rating, string Text, DateTime DateRated);
}