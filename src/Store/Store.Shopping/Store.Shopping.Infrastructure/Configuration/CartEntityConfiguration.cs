using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Shopping.Infrastructure.Entity;

namespace Store.Shopping.Infrastructure.Configuration;

public class CartEntityConfiguration : IEntityTypeConfiguration<CartEntity>
{
    public void Configure(EntityTypeBuilder<CartEntity> builder)
    {
        builder.ToTable("cart");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedOnAdd();
            
        builder.Property(c => c.CustomerNumber)
            .HasColumnName("customer_number")
            .IsRequired();
            
        builder.Property(c => c.SessionId)
            .HasColumnName("session_id")
            .IsRequired();
            
        builder.HasIndex(c => new { c.CustomerNumber, c.SessionId });
            
        builder.Property(c => c.CreatedAt).HasColumnName("created_at");
        builder.Property(c => c.UpdatedAt).HasColumnName("updated_at");

        builder.Property(c => c.Data)
            .HasColumnName("data")
            .HasColumnType("jsonb");
    }
}