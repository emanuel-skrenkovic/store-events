using System;
using Store.Core.Domain.Event;

namespace Store.Catalogue.Domain.Product.Events;

public record ProductRenamedEvent(Guid EntityId, string NewName) : IEvent;