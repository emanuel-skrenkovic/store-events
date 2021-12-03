using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Catalogue.Infrastructure.Entity;

namespace Store.Catalogue.Infrastructure.Configuration
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<ProductEntity>
    {
        public void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            builder.ToTable("product");
            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");

            builder.Property(p => p.CreatedAt).HasColumnName("created_at");
            builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");

            builder.Property(p => p.Description).HasColumnName("description");
            builder.Property(p => p.Name).HasColumnName("name");
            builder.Property(p => p.Available).HasColumnName("available");
            builder.Property(p => p.Price).HasColumnName("price");
        }
    }
}