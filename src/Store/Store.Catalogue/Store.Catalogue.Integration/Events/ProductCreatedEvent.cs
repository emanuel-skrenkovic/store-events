using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events;

public record ProductCreatedEvent(Guid ProductId, ProductView ProductView) : IEvent;