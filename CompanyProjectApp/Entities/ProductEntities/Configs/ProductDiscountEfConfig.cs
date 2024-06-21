using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ProductEntities.Configs;

public class ProductDiscountEfConfig : IEntityTypeConfiguration<ProductDiscount>
{
    public void Configure(EntityTypeBuilder<ProductDiscount> builder)
    {
        builder
            .HasKey(p => new { p.IdProduct, p.IdDiscount })
            .HasName("ProductDiscount_pk");

        builder
            .Property(p => p.IdDiscount)
            .ValueGeneratedNever();

        builder
            .Property(p => p.IdProduct)
            .ValueGeneratedNever();

        builder
            .HasOne(p => p.Discount)
            .WithMany(p => p.ProductDiscounts)
            .HasForeignKey(p => p.IdDiscount)
            .HasConstraintName("ProductDiscount_Discount_fk")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne(p => p.Product)
            .WithMany(p => p.ProductDiscounts)
            .HasForeignKey(p => p.IdProduct)
            .HasConstraintName("ProductDiscount_Product_fk")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .ToTable("Product_Discount");
    }
}