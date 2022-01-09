using Store.Core.Domain.Event;

namespace Store.Inventory.Domain.Events;

public record ProductInventoryAddedToStockEvent(Guid ProductId, int Count) : IEvent;