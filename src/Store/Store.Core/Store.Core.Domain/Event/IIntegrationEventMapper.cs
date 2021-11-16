namespace Store.Core.Domain.Event
{
    public interface IIntegrationEventMapper
    {
        bool TryMap(IEvent domainEvent, out IEvent integrationEvent);
    }
}