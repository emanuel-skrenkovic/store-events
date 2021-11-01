using Microsoft.EntityFrameworkCore;

namespace Store.Catalogue.Infrastructure.EntityFramework
{
    public class StoreCatalogueDbContext : DbContext
    {
        public StoreCatalogueDbContext(DbContextOptions<StoreCatalogueDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreCatalogueDbContext).Assembly);
        }
    }
}