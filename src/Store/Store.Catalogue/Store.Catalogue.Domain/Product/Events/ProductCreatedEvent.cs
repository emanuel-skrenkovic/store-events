using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductCreatedEvent(string Name, decimal Price, string Description = null) : IEvent;
}