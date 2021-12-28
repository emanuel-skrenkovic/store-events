using System.Threading.Tasks;
using Store.Core.Domain.ErrorHandling;

namespace Store.Core.Domain;

public interface IAggregateRepository
{
    Task<Result<T>> GetAsync<T, TKey>(TKey id)
        where T : AggregateEntity<TKey>, new();

    Task<Result> SaveAsync<T, TKey>(T entity)
        where T : AggregateEntity<TKey>;
}