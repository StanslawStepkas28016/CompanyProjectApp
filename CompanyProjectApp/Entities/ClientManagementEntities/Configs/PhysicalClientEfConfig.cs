using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ClientManagementEntities.Configs;

public class PhysicalClientEfConfig : IEntityTypeConfiguration<PhysicalClient>
{
    public void Configure(EntityTypeBuilder<PhysicalClient> builder)
    {
        builder
            .HasKey(pc => pc.IdPhysicalClient)
            .HasName("PhysicalClient_pk");

        builder
            .Property(pc => pc.IdPhysicalClient)
            .ValueGeneratedOnAdd();

        builder
            .Property(pc => pc.Pesel)
            .IsRequired();

        builder
            .Property(pc => pc.Name)
            .IsRequired();

        builder
            .Property(pc => pc.Surname)
            .IsRequired();

        builder
            .Property(pc => pc.Email)
            .IsRequired();

        builder
            .Property(pc => pc.PhoneNumber)
            .IsRequired();

        builder
            .Property(pc => pc.IsDeleted)
            .IsRequired();

        builder
            .ToTable("PhysicalClient");
    }
}