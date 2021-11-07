using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductMarkedUnavailableEvent(Guid EntityId) : IEvent;
}