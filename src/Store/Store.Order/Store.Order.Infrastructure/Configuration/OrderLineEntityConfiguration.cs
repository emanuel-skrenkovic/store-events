using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class OrderLineEntityConfiguration : IEntityTypeConfiguration<OrderLineEntity>
    {
        public void Configure(EntityTypeBuilder<OrderLineEntity> builder)
        {
            builder.ToTable("order_line");

            builder.HasKey(ol => ol.Id);
            builder.Property(ol => ol.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(ol => ol.ProductCatalogueNumber)
                .HasColumnName("product_catalogue_number")
                .IsRequired();
            builder.HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductCatalogueNumber);

            builder.Property(ol => ol.Count).HasColumnName("count");
            builder.Property(ol => ol.TotalAmount).HasColumnName("total_amount");

            builder.Property(ol => ol.OrderId).HasColumnName("order_id");
        }
    }
}