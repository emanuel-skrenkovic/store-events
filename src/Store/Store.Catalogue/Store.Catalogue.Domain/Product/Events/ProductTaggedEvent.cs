using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductTaggedEvent(Tag Tag) : IEvent;
}