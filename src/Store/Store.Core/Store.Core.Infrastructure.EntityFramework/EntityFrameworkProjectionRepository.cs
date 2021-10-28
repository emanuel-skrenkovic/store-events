using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;
using Store.Core.Domain.Projection;

namespace Store.Core.Infrastructure.EntityFramework
{
    public class EntityFrameworkProjectionRepository<TContext> : IProjectionRepository
        where TContext : DbContext
    {
        private readonly TContext _context;
        
        public EntityFrameworkProjectionRepository(TContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // TODO: should I use ValueTask<T> here?
        public async Task<T> GetAsync<T>(Guid id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public Task InsertAsync<T>(T entity) where T : class
        {
            Guard.IsNotNull(entity, nameof(entity));
            
            _context.Set<T>().Add(entity);

            return _context.SaveChangesAsync();
        }

        public Task UpdateAsync<T>(T entity) where T : class
        {
            Guard.IsNotNull(entity, nameof(entity));
            
            _context.Set<T>().Update(entity);

            return _context.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync<T>(Guid id) where T : class
        {
            T entity = await GetAsync<T>(id);
            if (entity == null) return;

            _context.Set<T>().Remove(entity);
            
            await _context.SaveChangesAsync();
        }
    }
}