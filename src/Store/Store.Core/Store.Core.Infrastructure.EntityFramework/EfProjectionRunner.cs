using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class EfProjectionRunner<TEntity, TContext> : IProjectionRunner
        where TEntity : ProjectionDocument, new()
        where TContext : DbContext
    {
        private readonly TContext _context;
        private readonly ISerializer _serializer;

        public EfProjectionRunner(TContext context, ISerializer serializer)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task RunAsync<T>(IProjection<T> projection, IEvent @event)
            where T : class, new()
        {
            Guard.IsNotNull(projection, nameof(projection));
            Guard.IsNotNull(@event, nameof(@event));

            DbSet<TEntity> set = _context.Set<TEntity>();

            TEntity entity = await set.FindAsync(@event.EntityId);

            if (entity == null)
            {
                entity = new();
                set.Add(entity);
            }

            // TODO: ugly and bad
            T updatedModel = projection.Project(_serializer.Deserialize<T>(entity.Data), @event);
            entity.Data = _serializer.Serialize(updatedModel);

            _context.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}