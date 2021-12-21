using System.Threading.Tasks;

namespace Store.Core.Domain;

public interface IAggregateRepository
{
    Task<T> GetAsync<T, TKey>(TKey id)
        where T : AggregateEntity<TKey>, new();

    Task SaveAsync<T, TKey>(T entity)
        where T : AggregateEntity<TKey>;
}