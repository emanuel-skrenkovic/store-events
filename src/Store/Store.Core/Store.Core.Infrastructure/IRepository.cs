using System;
using System.Threading.Tasks;
using Store.Core.Domain;

namespace Store.Core.Infrastructure
{
    public interface IRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : AggregateEntity, new();

        Task SaveAsync<T>(T entity) where T : AggregateEntity;
    }
}