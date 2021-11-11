using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class OrderDisplayEntityConfiguration : IEntityTypeConfiguration<OrderDisplayEntity>
    {
        public void Configure(EntityTypeBuilder<OrderDisplayEntity> builder)
        {
            builder.ToTable("order_display");
            
            builder.HasKey(o => o.Id);
            builder.Property(o => o.Id).HasColumnName("id");
            
            builder.Ignore(o => o.CreatedAt);
            builder.Ignore(o => o.UpdatedAt);

            builder.Ignore(o => o.CustomerNumber);
            builder.Ignore(o => o.OrderLines);
            builder.Ignore(o => o.ShippingInformation);
                
            builder.Property(p => p.Data).HasColumnName("data").HasColumnType("jsonb");
        }
    }
}