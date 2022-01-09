using Store.Core.Domain.Event;

namespace Store.Inventory.Domain.Events;

public record ProductInventoryRemovedFromStockEvent(Guid ProductId, int Count) : IEvent;