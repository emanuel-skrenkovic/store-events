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

        public Task RunAsync<T>(IProjectionOperation<T> operation) where T : class
        {
            Guard.IsNotNull(operation, nameof(operation));
            
            return _repository.InsertAsync(operation.Apply());
        }

        public async Task RunUpdateAsync<T>(IProjectionUpdateOperation<T> operation) where T : class
        {
            Guard.IsNotNull(operation, nameof(operation));

            T model = await _repository.GetAsync<T>(Guid.Empty);
            await _repository.UpdateAsync(operation.ApplyUpdate(model));
        }
    }
}