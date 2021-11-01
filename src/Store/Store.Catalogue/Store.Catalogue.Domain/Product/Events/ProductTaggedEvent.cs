using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events
{
    public record ProductTaggedEvent(Guid EntityId, Tag Tag) : IEvent;
}