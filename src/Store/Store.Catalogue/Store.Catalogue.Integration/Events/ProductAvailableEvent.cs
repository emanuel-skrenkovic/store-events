using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events;

public record ProductAvailableEvent(Guid ProductId) : IEvent;