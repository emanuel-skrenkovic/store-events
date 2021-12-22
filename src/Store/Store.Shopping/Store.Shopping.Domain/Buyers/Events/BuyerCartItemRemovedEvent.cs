using Store.Core.Domain.Event;

namespace Store.Shopping.Domain.Buyers.Events;

public record BuyerCartItemRemovedEvent(string BuyerId, string ProductCatalogueNumber) : IEvent;