using Store.Core.Domain;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductPriceChangedEvent(decimal NewPrice, string reason = null): IEvent;
}