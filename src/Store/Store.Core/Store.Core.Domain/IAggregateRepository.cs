using System;
using System.Threading.Tasks;

namespace Store.Core.Domain
{
    public interface IAggregateRepository
    {
        Task<T> GetAsync<T, TKey>(TKey id) 
            where T : AggregateEntity<TKey>, new()
            where TKey : struct;
        
        Task CreateAsync<T, TKey>(T entity) 
            where T : AggregateEntity<TKey>
            where TKey : struct;

        Task SaveAsync<T, TKey>(T entity) 
            where T : AggregateEntity<TKey>
            where TKey : struct;
    }
}