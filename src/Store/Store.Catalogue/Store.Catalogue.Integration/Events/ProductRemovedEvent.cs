using Store.Core.Domain.Event;

namespace Store.Catalogue.Integration.Events;

public record ProductRemovedEvent(Guid ProductId) : IEvent;