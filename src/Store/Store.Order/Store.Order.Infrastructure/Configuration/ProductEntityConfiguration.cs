using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration;

public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
{
    public void Configure(EntityTypeBuilder<ProductEntity> builder)
    {
        builder.ToTable("product");
            
        builder.HasKey(p => p.CatalogueNumber);
        builder.Property(p => p.CatalogueNumber).HasColumnName("catalogue_number");
            
        builder.Property(p => p.CreatedAt).HasColumnName("created_at");
        builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

        builder.Property(p => p.Name).HasColumnName("name");
        builder.Property(p => p.Price).HasColumnName("price");
        builder.Property(p => p.Available).HasColumnName("available");
    }
}