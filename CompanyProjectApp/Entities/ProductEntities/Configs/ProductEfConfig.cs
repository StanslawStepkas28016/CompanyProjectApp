using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ProductEntities.Configs;

public class ProductEfConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .HasKey(p => p.IdProduct)
            .HasName("Product_pk");

        builder
            .Property(p => p.IdProduct)
            .ValueGeneratedOnAdd();

        builder
            .Property(p => p.Name)
            .IsRequired();

        builder
            .Property(p => p.Description)
            .IsRequired();

        builder
            .Property(p => p.VersionInfo)
            .IsRequired();

        builder
            .Property(p => p.Category)
            .IsRequired();

        builder
            .Property(p => p.Price)
            .IsRequired();

        builder
            .ToTable("Product");
    }
}