using System;
using Store.Core.Domain.Event;

namespace Store.Order.Domain.Buyers.Events
{
    public record BuyerCartItemRemovedEvent(string EntityId, Item Item) : IEvent;
}