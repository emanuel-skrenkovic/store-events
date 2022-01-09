using Store.Core.Domain.Event;

namespace Store.Inventory.Domain.Events;

public record ProductInventoryCreatedEvent(Guid ProductId) : IEvent;