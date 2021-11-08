using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Buyers.Events
{
    public record BuyerCartItemAddedEvent(Guid EntityId, Item Item) : IEvent;
}