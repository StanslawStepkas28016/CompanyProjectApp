using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CompanyProjectApp.Entities.AppUserEntities.Configs;

public class AppUserEfConfig : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder
            .HasKey(au => au.Login)
            .HasName("AppUser_pk");

        builder
            .Property(cc => cc.Login)
            .ValueGeneratedNever();

        builder
            .Property(cc => cc.Password)
            .IsRequired();

        builder
            .Property(cc => cc.Role)
            .IsRequired();

        builder
            .Property(cc => cc.Salt)
            .IsRequired();

        builder
            .Property(cc => cc.RefreshToken)
            .IsRequired();

        builder
            .Property(cc => cc.RefreshTokenExp)
            .IsRequired();

        builder
            .ToTable("AppUser");
    }
}