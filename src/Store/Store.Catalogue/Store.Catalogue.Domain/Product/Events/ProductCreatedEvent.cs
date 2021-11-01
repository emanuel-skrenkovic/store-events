using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductCreatedEvent(Guid EntityId, string Name, decimal Price, string Description = null) : IEvent;
}