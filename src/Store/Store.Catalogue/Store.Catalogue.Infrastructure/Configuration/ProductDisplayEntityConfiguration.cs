using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Catalogue.Infrastructure.Entity;

namespace Store.Catalogue.Infrastructure.Configuration
{
    public class ProductDisplayEntityConfiguration : IEntityTypeConfiguration<ProductDisplayEntity>
    {
        public void Configure(EntityTypeBuilder<ProductDisplayEntity> builder)
        {
            builder.ToTable("product_display");
            
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnName("id");

            builder.Ignore(p => p.Description);
            builder.Ignore(p => p.Name);
            builder.Ignore(p => p.Price);
            builder.Ignore(p => p.Reviews);
            builder.Ignore(p => p.Tags);

            builder.Property(p => p.Serialized).HasColumnName("data").HasColumnType("jsonb");
        }
    }
}