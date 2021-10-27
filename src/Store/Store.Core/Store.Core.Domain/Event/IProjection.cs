namespace Store.Core.Domain.Event
{
    public interface IProjection
    {
        void Handle(object integrationEvent);
    }
}