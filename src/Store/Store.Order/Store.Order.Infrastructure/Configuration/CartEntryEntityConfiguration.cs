using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class CartEntryEntityConfiguration : IEntityTypeConfiguration<CartEntryEntity>
    {
        public void Configure(EntityTypeBuilder<CartEntryEntity> builder)
        {
            builder.ToTable("cart_entry");

            builder.HasKey(c => new { c.CustomerNumber, c.SessionId });
            
            builder.Property(c => c.CustomerNumber)
                .HasColumnName("customer_number")
                .IsRequired();
            builder.Property(c => c.SessionId)
                .HasColumnName("session_id")
                .IsRequired();
            
            builder.Property(c => c.CreatedAt).HasColumnName("created_at");
            builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");

            builder.Property(c => c.ProductCatalogueNumber)
                .HasColumnName("product_catalogue_number")
                .IsRequired();
            builder.HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductCatalogueNumber);
            
            builder.Property(c => c.Quantity).HasColumnName("quantity");
        }
    }
}