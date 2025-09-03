using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.ClientManagementEntities.Configs;

public class CompanyClientEfConfig : IEntityTypeConfiguration<CompanyClient>
{
    public void Configure(EntityTypeBuilder<CompanyClient> builder)
    {
        builder
            .HasKey(cc => cc. IdCompanyClient)
            .HasName("CompanyClient_pk");

        builder
            .Property(cc => cc.IdCompanyClient)
            .ValueGeneratedOnAdd();

        builder
            .Property(cc => cc.KrsNumber)
            .IsRequired();

        builder
            .Property(cc => cc.KrsNumber)
            .IsRequired();

        builder
            .Property(cc => cc.Address)
            .IsRequired();

        builder
            .Property(cc => cc.Email)
            .IsRequired();

        builder
            .Property(cc => cc.PhoneNumber)
            .IsRequired();

        builder
            .ToTable("CompanyClient");
    }
}