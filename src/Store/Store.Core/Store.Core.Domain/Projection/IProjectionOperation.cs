namespace Store.Core.Domain.Projection
{
    public interface IProjectionOperation<TModel>
    {
        TModel Apply();
    }

    public interface IProjectionUpdateOperation<TModel>
    {
        TModel ApplyUpdate(TModel model);
    }
}