using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class CartItemEntityConfiguration : IEntityTypeConfiguration<CartEntryEntity>
    {
        public void Configure(EntityTypeBuilder<CartEntryEntity> builder)
        {
            builder.HasNoKey();
            builder.Ignore(i => i.Id);

            builder.Ignore(i => i.Quantity);
        }
    }
}