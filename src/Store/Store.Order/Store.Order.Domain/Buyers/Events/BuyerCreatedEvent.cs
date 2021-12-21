using Store.Core.Domain.Event;

namespace Store.Order.Domain.Buyers.Events;

public record BuyerCreatedEvent(string CustomerNumber, string SessionId) : IEvent;