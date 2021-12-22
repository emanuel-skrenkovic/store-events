using Store.Core.Domain.Event;

namespace Store.Shopping.Domain.Buyers.Events;

public record BuyerCartItemAddedEvent(string BuyerId, string ProductCatalogueNumber) : IEvent;