using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductRatedEvent(ProductRating ProductRating) : IEvent;
}