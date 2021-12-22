using Store.Core.Domain.Event;

namespace Store.Shopping.Domain.Buyers.Events;

public record BuyerCreatedEvent(string CustomerNumber, string SessionId) : IEvent;