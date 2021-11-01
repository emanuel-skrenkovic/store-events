using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Catalogue.Domain.Product.Projections.ProductDisplay;

namespace Store.Catalogue.Infrastructure.EntityFramework.Configuration
{
    public class ProductConfiguration : IEntityTypeConfiguration<ProductDisplay>
    {
        public void Configure(EntityTypeBuilder<ProductDisplay> builder)
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