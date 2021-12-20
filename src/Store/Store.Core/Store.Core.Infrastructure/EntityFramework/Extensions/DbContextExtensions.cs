using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Infrastructure.EntityFramework.Entity;

namespace Store.Core.Infrastructure.EntityFramework.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<ulong> GetSubscriptionCheckpoint(this DbContext context, string subscriptionId)
        {
            SubscriptionCheckpointEntity checkpoint = await context.Set<SubscriptionCheckpointEntity>()
                .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

            return checkpoint?.Position ?? 0;
        }

        public static async Task AddOrUpdateSubscriptionCheckpoint(this DbContext context, string subscriptionId, ulong newPosition)
        {
            SubscriptionCheckpointEntity checkpoint = await context.Set<SubscriptionCheckpointEntity>()
                .FirstOrDefaultAsync(c => c.SubscriptionId == subscriptionId);

            if (checkpoint == null)
            {
                checkpoint = new() { Id = Guid.NewGuid(), SubscriptionId = subscriptionId, Position = newPosition };
                context.Add(checkpoint);
            }
            else
            {
                checkpoint.Position = newPosition;
                context.Update(checkpoint);
            }
        }
    }
}