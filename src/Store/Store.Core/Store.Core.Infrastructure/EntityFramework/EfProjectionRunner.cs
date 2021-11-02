using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Domain.Event;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class EfProjectionRunner<TModel, TContext> : IProjectionRunner<TModel> 
        where TModel : class, IProjectionDocument, IReadModel, new()
        where TContext : DbContext
    {
        private readonly ISerializer _serializer;
        private readonly IServiceScopeFactory _scopeFactory;

        public EfProjectionRunner(ISerializer serializer, IServiceScopeFactory scopeFactory)
        {
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }
        
        public async Task RunAsync(IProjection<TModel> projection, IEvent @event)
        {
            Guard.IsNotNull(@event, nameof(@event));

            using IServiceScope scope = _scopeFactory.CreateScope();

            TContext context = scope.ServiceProvider.GetRequiredService<TContext>();
            if (context == null) return;
            
            DbSet<TModel> set = context.Set<TModel>();

            TModel model = await set.FindAsync(@event.EntityId);

            bool isNew = model == null;
            if (isNew)
            {
                model = new() { Id = @event.EntityId };
            }
            else
            {
                model.DeserializeData(_serializer);
            }

            projection.Project(model, @event);
            model.SerializeData(_serializer);

            if (isNew)
            {
                set.Add(model);
            }
            else
            {
                set.Update(model);
            }
            
            await context.SaveChangesAsync();
        }
    }
}