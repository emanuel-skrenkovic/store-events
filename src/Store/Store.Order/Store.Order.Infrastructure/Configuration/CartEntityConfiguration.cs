using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class CartEntityConfiguration : IEntityTypeConfiguration<CartEntity>
    {
        public void Configure(EntityTypeBuilder<CartEntity> builder)
        {
            builder.ToTable("cart");
            
            builder.HasKey(c => c.Id);
            builder.Property(c => c.Id).HasColumnName("id");
            
            builder.Ignore(c => c.CreatedAt);
            builder.Ignore(c => c.UpdatedAt);

            builder.Ignore(c => c.Items);
                
            builder.Property(c => c.Data).HasColumnName("data").HasColumnType("jsonb");
        }
    }
}