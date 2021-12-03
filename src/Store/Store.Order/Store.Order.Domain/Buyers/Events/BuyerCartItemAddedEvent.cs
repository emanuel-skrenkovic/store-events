using Store.Core.Domain.Event;

namespace Store.Order.Domain.Buyers.Events
{
    public record BuyerCartItemAddedEvent(string BuyerId, string ProductCatalogueNumber) : IEvent;
}