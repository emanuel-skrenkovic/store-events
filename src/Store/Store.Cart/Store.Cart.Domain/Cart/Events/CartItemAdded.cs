using System;
using Store.Core.Domain.Event;

namespace Store.Cart.Domain.Events
{
    public record CartItemAdded(Guid EntityId, CartItem Item) : IEvent;
}