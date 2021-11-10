using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Store.Core.Domain;

namespace Store.Core.Infrastructure.EntityFramework.Extensions
{
    /// <summary>
    /// Helpers methods to deal with IProjectionDocument which holds all the data in jsonb in postgres.
    /// </summary>
    public static class DbContextExtensions
    {
        public static async Task<T> GetProjectionDocumentAsync<T>(this DbContext context, ISerializer serializer, object id) where T : class, IProjectionDocument
        {
            T model = await context.Set<T>().FindAsync(id);
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
    }
}