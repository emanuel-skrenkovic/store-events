using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events;

public record ProductPriceChangedEvent(Guid EntityId, decimal NewPrice, string reason = null): IEvent;