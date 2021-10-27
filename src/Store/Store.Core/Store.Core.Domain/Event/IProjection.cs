namespace Store.Core.Domain.Event
{
    public interface IProjection<TProjection>
    {
        void Handle(object integrationEvent);
    }
}