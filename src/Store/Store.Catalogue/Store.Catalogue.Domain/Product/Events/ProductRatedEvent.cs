using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductRatedEvent(Guid EntityId, ProductRating ProductRating) : IEvent;
}