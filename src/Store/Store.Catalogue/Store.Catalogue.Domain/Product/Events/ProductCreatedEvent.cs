using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductCreatedEvent(string Name, decimal Price, string Description = null) : IEvent;
}