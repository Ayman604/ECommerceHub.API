using ECommerce.Domain.Entities.OrderModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.Presistence.Data.Configurations
{
    internal class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(X => X.SubTotal).HasColumnType("decimal(8,2)");
            builder.OwnsOne(X => X.Address,
                OE =>
                {
                    OE.Property(X => X.FirstName).HasMaxLength(50);
                    OE.Property(X => X.LastName).HasMaxLength(50);
                    OE.Property(X => X.City).HasMaxLength(50);
                    OE.Property(X => X.Street).HasMaxLength(50);
                    OE.Property(X => X.Country).HasMaxLength(50);
                });
        }
    }
}
