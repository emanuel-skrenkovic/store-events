using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class EfProjectionRunner<TContext> : IProjectionRunner where TContext : DbContext
    {
        private readonly TContext _context;

        public EfProjectionRunner(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task RunAsync<T>(IProjection<T> projection, IEvent @event)
            where T : class, new()
        {
            Guard.IsNotNull(projection, nameof(projection));
            Guard.IsNotNull(@event, nameof(@event));

            DbSet<T> set = _context.Set<T>();

            T entity = await set.FindAsync(@event.EntityId);

            bool isNew = entity == null;
            if (isNew)   entity = new();

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
            
            await _context.SaveChangesAsync();
        }
    }
}