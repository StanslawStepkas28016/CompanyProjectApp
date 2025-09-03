using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ProductEntities.Configs;

public class DiscountEfConfig : IEntityTypeConfiguration<Discount>
{
    public void Configure(EntityTypeBuilder<Discount> builder)
    {
        builder
            .HasKey(d => d.IdDiscount)
            .HasName("Discount_pk");

        builder
            .Property(d => d.IdDiscount)
            .ValueGeneratedOnAdd();

        builder
            .Property(d => d.Name)
            .IsRequired();

        builder
            .Property(d => d.Name)
            .IsRequired();
        
        builder
            .ToTable("Discount");
    }
}