using System;

namespace Store.Catalogue.Application.Product
{
    public record ProductDto(Guid Id, string Name, decimal Price, string Description = null);
}