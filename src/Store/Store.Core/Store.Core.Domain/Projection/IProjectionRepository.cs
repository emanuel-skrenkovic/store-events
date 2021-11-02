using System;
using System.Threading.Tasks;

namespace Store.Core.Domain.Projection
{
    public interface IProjectionRepository
    {
        Task<T> GetAsync<T>(Guid id) where T : class;

        Task InsertAsync<T>(T entity) where T : class;

        Task UpdateAsync<T>(T entity) where T : class;

        Task DeleteByIdAsync<T>(Guid id) where T : class; // TODO: id generic type?
    }
}