using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events;

public record ProductUpdatedEvent(Guid ProductId, ProductView ProductView) : IEvent;