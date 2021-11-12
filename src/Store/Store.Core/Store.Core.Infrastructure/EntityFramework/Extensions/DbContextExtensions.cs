using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Infrastructure.EntityFramework.Entity;

namespace Store.Core.Infrastructure.EntityFramework.Extensions
{
    /// <summary>
    /// Helpers methods to deal with IProjectionDocument which holds all the data in jsonb in postgres.
    /// </summary>
    public static class DbContextExtensions
    {
        #region ProjectionHelpers
        
        public static async Task<T> GetProjectionDocumentAsync<T>(this DbContext context, ISerializer serializer, object id) where T : class, IProjectionDocument
        {
            T model = await context.Set<T>().FindAsync(id);
            if (model == null) return null;
            
            model.DeserializeData(serializer);

            return model;
        }

        public static async Task<T> GetProjectionDocumentByAsync<T>(this DbContext context, ISerializer serializer, Expression<Func<T, bool>> predicate) where T : class, IProjectionDocument
        {
            T model = await context.Set<T>().FirstOrDefaultAsync(predicate);
            if (model == null) return null;
            
            model.DeserializeData(serializer);

            return model; 
        }
        
        public static void UpdateProjectionDocument<T>(this DbContext context, ISerializer serializer, T model) where T : class, IProjectionDocument
        {
            model.SerializeData(serializer);
            context.Set<T>().Update(model);
        }
        
        public static void AddProjectionDocument<T>(this DbContext context, ISerializer serializer, T model) where T : class, IProjectionDocument
        {
            model.SerializeData(serializer);
            context.Set<T>().Add(model);
        }
        
        #endregion

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