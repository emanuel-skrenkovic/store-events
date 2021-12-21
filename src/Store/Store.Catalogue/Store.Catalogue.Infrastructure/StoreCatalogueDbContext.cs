using Microsoft.EntityFrameworkCore;
using Store.Catalogue.Infrastructure.Entity;
using Store.Core.Infrastructure.EntityFramework.Entity;

namespace Store.Catalogue.Infrastructure;

public class StoreCatalogueDbContext : DbContext
{
    public DbSet<ProductEntity> ProductDisplays { get; set; }
        
    public DbSet<SubscriptionCheckpointEntity> SubscriptionCheckpoint { get; set; }
        
    public StoreCatalogueDbContext(DbContextOptions<StoreCatalogueDbContext> options) : base(options) { }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");
            
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreCatalogueDbContext).Assembly);
            
        base.OnModelCreating(modelBuilder);
    }
}