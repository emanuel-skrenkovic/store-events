using System;
using System.Threading.Tasks;

namespace Store.Core.Domain
{
    public interface IRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : AggregateEntity, new();
        
        Task CreateAsync<T>(T entity) where T : AggregateEntity;

        Task SaveAsync<T>(T entity) where T : AggregateEntity;
    }
}