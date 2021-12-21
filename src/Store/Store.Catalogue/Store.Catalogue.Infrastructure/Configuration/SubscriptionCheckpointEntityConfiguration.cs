using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Core.Infrastructure.EntityFramework.Entity;

namespace Store.Catalogue.Infrastructure.Configuration;

public class SubscriptionCheckpointEntityConfiguration : IEntityTypeConfiguration<SubscriptionCheckpointEntity>
{
    public void Configure(EntityTypeBuilder<SubscriptionCheckpointEntity> builder)
    {
        builder.ToTable("subscription_checkpoint");

        builder.Property(c => c.Id).HasColumnName("id");
            
        builder.Property(c => c.SubscriptionId).HasColumnName("subscription_id");
        builder.HasIndex(c => c.SubscriptionId);
            
        builder.Property(c => c.Position).HasColumnName("position");
    }
}