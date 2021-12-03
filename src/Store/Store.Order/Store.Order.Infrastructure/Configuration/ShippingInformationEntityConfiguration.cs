using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Store.Order.Infrastructure.Entity;

namespace Store.Order.Infrastructure.Configuration
{
    public class ShippingInformationEntityConfiguration : IEntityTypeConfiguration<ShippingInformationEntity>
    {
        public void Configure(EntityTypeBuilder<ShippingInformationEntity> builder)
        {
            builder.ToTable("shipping_information");

            builder.HasKey(si => si.Id);
            builder.Property(si => si.Id).HasColumnName("id").ValueGeneratedOnAdd();

            builder.Property(si => si.CountryCode).HasColumnName("country_code");
            builder.Property(si => si.FullName).HasColumnName("full_name");
            builder.Property(si => si.StreetAddress).HasColumnName("street_address");
            builder.Property(si => si.City).HasColumnName("city");
            builder.Property(si => si.StateProvince).HasColumnName("state_province");
            builder.Property(si => si.Postcode).HasColumnName("postcode");
            builder.Property(si => si.PhoneNumber).HasColumnName("phone_number");
            builder.Property(si => si.OrderId).HasColumnName("order_id");
        }
    }
}