using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ProductEntities.Configs;

public class AgreementEfConfig : IEntityTypeConfiguration<Agreement>
{
    public void Configure(EntityTypeBuilder<Agreement> builder)
    {
        builder
            .HasKey(a => a.IdAgreement)
            .HasName("Agreement_pk");

        builder
            .Property(a => a.IdAgreement)
            .ValueGeneratedOnAdd();

        builder
            .Property(a => a.IdClient)
            .IsRequired();

        builder
            .Property(a => a.ClientType)
            .IsRequired();

        builder
            .Property(a => a.IdProduct)
            .IsRequired();

        builder
            .Property(a => a.ProductVersionInfo)
            .IsRequired();

        builder
            .Property(a => a.AgreementDateFrom)
            .IsRequired();

        builder
            .Property(a => a.AgreementDateTo)
            .IsRequired();

        builder
            .Property(a => a.CalculatedPrice)
            .IsRequired();

        builder
            .Property(a => a.ProductUpdatesToInYears)
            .IsRequired();

        builder
            .Property(a => a.IsSigned)
            .IsRequired();

        builder
            .HasOne(a => a.Product)
            .WithMany(a => a.Agreements)
            .HasForeignKey(a => a.IdProduct)
            .HasConstraintName("Agreement_Product_fk")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .ToTable("Agreement");
    }
}