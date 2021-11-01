using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Catalogue.Infrastructure.EntityFramework.Entity;

namespace Store.Catalogue.Infrastructure.EntityFramework.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductDisplayEntity>
    {
        public void Configure(EntityTypeBuilder<ProductDisplayEntity> builder)
        {
            builder.ToTable("product_display");
            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            
            builder.Property(p => p.CreatedAt).HasColumnName("created_at");
            builder.Property(p => p.UpdatedAt).HasColumnName("updated_at");
            
            builder.Property(p => p.Data).HasColumnName("data").HasColumnType("jsonb");
        }
    }
}