using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ProductEntities.Configs;

public class PaymentEfConfig : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder
            .HasKey(p => p.IdPayment)
            .HasName("Payment_pk");

        builder
            .Property(p => p.IdPayment)
            .ValueGeneratedOnAdd();

        builder
            .Property(p => p.IdAgreement)
            .IsRequired();

        builder
            .Property(p => p.MoneyOwed)
            .IsRequired();

        builder
            .Property(p => p.MoneyPaid)
            .IsRequired();

        builder
            .HasOne(p => p.Agreement)
            .WithMany(p => p.Payments)
            .HasForeignKey(p => p.IdAgreement)
            .HasConstraintName("Payment_Agreement_fk")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .ToTable("Payment");
    }
}