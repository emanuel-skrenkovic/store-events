using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events
{
    public record ProductPriceChangedEvent(Guid ProductId, decimal NewPrice) : IIntegrationEvent;
}