using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Store.Core.Domain;
using Store.Core.Infrastructure.EntityFramework.Entity;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class EfCheckpointRepository<TContext> : ICheckpointRepository where TContext : DbContext
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public EfCheckpointRepository(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        }

        public async Task<ulong?> GetAsync(string subscriptionId)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            TContext context = scope.ServiceProvider.GetService<TContext>();
            
            SubscriptionCheckpointEntity checkpoint = await context
                .Set<SubscriptionCheckpointEntity>()
                .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

            return checkpoint?.Position ?? 0;
        }

        public async Task SaveAsync(string subscriptionId, ulong position)
        {
            using IServiceScope scope = _scopeFactory.CreateScope();
            TContext context = scope.ServiceProvider.GetService<TContext>();
            
            DbSet<SubscriptionCheckpointEntity> set = context.Set<SubscriptionCheckpointEntity>();
            SubscriptionCheckpointEntity entity = await set.FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

            if (entity == null)
            {
                entity = new() { SubscriptionId = subscriptionId, Position = position };
                set.Add(entity);
            }
            else
            {
                entity.Position = position;
                set.Update(entity);
            }

            await context.SaveChangesAsync();
        }
    }
}