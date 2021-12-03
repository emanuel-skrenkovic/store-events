using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class OrderEntityConfiguration : IEntityTypeConfiguration<OrderEntity>
    {
        public void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.ToTable("order");
            
            builder.HasKey(o => o.OrderId);
            builder.Property(o => o.OrderId).HasColumnName("order_id");
            
            builder.Property(o => o.CreatedAt).HasColumnName("created_at");
            builder.Property(o => o.UpdatedAt).HasColumnName("updated_at");

            builder.Property(o => o.CustomerNumber).HasColumnName("customer_number");
            builder.Property(o => o.TotalAmount).HasColumnName("total_amount");

            builder.HasMany(o => o.OrderLines)
                .WithOne(ol => ol.OrderEntity)
                .HasForeignKey(ol => ol.OrderId);

            builder.HasOne(o => o.ShippingInformation)
                .WithOne()
                .HasForeignKey<ShippingInformationEntity>(si => si.OrderId);
        }
    }
}