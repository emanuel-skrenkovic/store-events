using Microsoft.EntityFrameworkCore;
using Store.Core.Infrastructure.EntityFramework.Entity;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Infrastructure;

public class StoreShoppingDbContext : DbContext
{
    public DbSet<OrderEntity> Orders { get; set; }
        
    public DbSet<CartEntity> Carts { get; set; }
        
    public DbSet<ProductEntity> Products { get; set; }
        
    public DbSet<SubscriptionCheckpointEntity> SubscriptionCheckpoint { get; set; }

    public StoreShoppingDbContext(DbContextOptions<StoreShoppingDbContext> options) : base(options) { }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("public");

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreShoppingDbContext).Assembly);
            
        base.OnModelCreating(modelBuilder);
    }
}