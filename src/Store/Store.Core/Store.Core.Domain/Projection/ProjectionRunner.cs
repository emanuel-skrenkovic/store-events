using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Projection
{
    public class ProjectionRunner : IProjectionRunner
    {
        private readonly IProjectionRepository _repository;

        public ProjectionRunner(IProjectionRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task RunAsync<T>(IProjection<T> projection, object @event) where T : class, new()
        {
            Guard.IsNotNull(projection, nameof(projection));

            T model = await _repository.GetAsync<T>(Guid.Empty);

            bool isNew = model == null;
            if (isNew)
            {
                model = new();
            }

            T projectedModel = projection.Project(model, @event);


            if (isNew)
            {
                await _repository.InsertAsync(projectedModel);
            }
            else
            {
                await _repository.UpdateAsync(projectedModel);
            }
        }
    }
}