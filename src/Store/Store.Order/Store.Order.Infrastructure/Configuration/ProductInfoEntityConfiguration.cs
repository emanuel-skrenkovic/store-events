using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class ProductInfoEntityConfiguration : IEntityTypeConfiguration<ProductInfoEntity>
    {
        public void Configure(EntityTypeBuilder<ProductInfoEntity> builder)
        {
            builder.ToTable("product_info");
            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");
            
            builder.Ignore(p => p.CreatedAt);
            builder.Ignore(p => p.UpdatedAt);

            builder.Ignore(p => p.Name);
            builder.Ignore(p => p.Price);
            builder.Ignore(p => p.Available);
                
            builder.Property(p => p.Data).HasColumnName("data").HasColumnType("jsonb");
        }
    }
}