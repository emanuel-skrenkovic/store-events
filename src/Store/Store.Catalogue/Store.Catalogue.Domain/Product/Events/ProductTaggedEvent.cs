using Store.Core.Domain;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductTaggedEvent(Tag Tag) : IEvent;
}