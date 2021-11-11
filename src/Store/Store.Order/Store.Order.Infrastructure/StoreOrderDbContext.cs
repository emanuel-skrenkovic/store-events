using Microsoft.EntityFrameworkCore;
using Store.Core.Infrastructure.EntityFramework.Entity;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure
{
    public class StoreOrderDbContext : DbContext
    {
        public DbSet<OrderDisplayEntity> ProductDisplays { get; set; }
        
        public DbSet<SubscriptionCheckpointEntity> SubscriptionCheckpoint { get; set; }

        public StoreOrderDbContext(DbContextOptions<StoreOrderDbContext> options) : base(options) { }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Database.Migrate();
            modelBuilder.HasDefaultSchema("public");
            
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(StoreOrderDbContext).Assembly);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}