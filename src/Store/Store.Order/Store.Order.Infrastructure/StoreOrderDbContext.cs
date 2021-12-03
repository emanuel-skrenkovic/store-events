using Microsoft.EntityFrameworkCore;
using Store.Core.Infrastructure.EntityFramework.Entity;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure
{
    public class StoreOrderDbContext : DbContext
    {
        public DbSet<OrderEntity> Orders { get; set; }
        
        public DbSet<OrderLineEntity> OrderLines { get; set; }
        
        public DbSet<CartEntryEntity> CartEntries { get; set; }
        
        public DbSet<ProductEntity> Products { get; set; }
        
        public DbSet<SubscriptionCheckpointEntity> SubscriptionCheckpoint { get; set; }

        public StoreOrderDbContext(DbContextOptions<StoreOrderDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreOrderDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}