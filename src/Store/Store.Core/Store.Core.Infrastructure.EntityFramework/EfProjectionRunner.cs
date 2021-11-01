using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class EfProjectionRunner<TContext> : IProjectionRunner where TContext : DbContext
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public EfProjectionRunner(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task RunAsync<T>(IProjection<T> projection, IEvent @event)
            where T : class, IReadModel, new()
        {
            Guard.IsNotNull(projection, nameof(projection));
            Guard.IsNotNull(@event, nameof(@event));

            using IServiceScope scope = _serviceScopeFactory.CreateScope();
            TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
            DbSet<T> set = context.Set<T>();

            T entity = await set.FindAsync(@event.EntityId);

            bool isNew = entity == null;
            if (isNew)
            {
                entity = new();
                entity.Id = @event.EntityId;
            }

            // TODO: ugly and bad
            T updatedEntity = projection.Project(entity, @event);

            if (isNew)
            {
                set.Add(updatedEntity);
            }
            else
            {
                set.Update(updatedEntity);
            }
            
            await context.SaveChangesAsync();
        }
    }
}