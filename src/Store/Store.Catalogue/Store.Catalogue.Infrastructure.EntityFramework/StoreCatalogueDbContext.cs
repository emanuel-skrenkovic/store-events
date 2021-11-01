using Microsoft.EntityFrameworkCore;
using Store.Catalogue.Infrastructure.EntityFramework.Entity;

namespace Store.Catalogue.Infrastructure.EntityFramework
{
    public class StoreCatalogueDbContext : DbContext
    {
        public DbSet<ProductDisplayEntity> ProductDisplays { get; set; }
        
        public StoreCatalogueDbContext(DbContextOptions<StoreCatalogueDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreCatalogueDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}