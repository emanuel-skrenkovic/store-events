using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductRatedEvent(ProductRating ProductRating) : IEvent;
}