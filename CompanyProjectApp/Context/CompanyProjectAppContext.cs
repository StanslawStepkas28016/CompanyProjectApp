using CompanyProjectApp.Entities.AppUserEntities;
using CompanyProjectApp.Entities.ClientManagementEntities;
using CompanyProjectApp.Entities.ProductEntities;
using Microsoft.EntityFrameworkCore;

namespace CompanyProjectApp.Context;

public class CompanyProjectAppContext : DbContext
{
    public virtual DbSet<PhysicalClient> PhysicalClients { get; set; }

    public virtual DbSet<CompanyClient> CompanyClients { get; set; }

    public virtual DbSet<AppUser> AppUsers { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<Agreement> Agreements { get; set; }

    public virtual DbSet<Discount> Discounts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<ProductDiscount> ProductDiscounts { get; set; }

    public CompanyProjectAppContext(DbContextOptions<CompanyProjectAppContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Fabrykacja modelu.
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CompanyProjectAppContext).Assembly);

        if (Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer")
        {
            // Seeding, wprowadzenie danych. 
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Login = "admin",
                    Password = "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=",
                    Role = "admin",
                    Salt = "fUYAuxqW54H8a69VlzikJg==",
                    RefreshToken = "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=",
                    RefreshTokenExp = new DateTime(2024, 6, 20)
                },
                new AppUser
                {
                    Login = "worker1",
                    Password = "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=",
                    Role = "regular",
                    Salt = "fUYAuxqW54H8a69VlzikJg==",
                    RefreshToken = "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=",
                    RefreshTokenExp = new DateTime(2024, 6, 20)
                },
                new AppUser
                {
                    Login = "worker2",
                    Password = "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=",
                    Role = "asda231dasda",
                    Salt = "fUYAuxqW54H8a69VlzikJg==",
                    RefreshToken = "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=",
                    RefreshTokenExp = new DateTime(2024, 6, 20)
                }
            );

            modelBuilder.Entity<CompanyClient>().HasData(
                new CompanyClient
                {
                    IdCompanyClient = 1,
                    KrsNumber = "113021392",
                    Email = "weloveit@weloveit.pl",
                    Address = "Koszykowa 13, Warszawa",
                    PhoneNumber = 696784867
                },
                new CompanyClient
                {
                    IdCompanyClient = 2,
                    KrsNumber = "443051392",
                    Email = "itforu@itforu.pl",
                    Address = "Kwiatowa 10, Warszawa",
                    PhoneNumber = 539981155
                }
            );

            modelBuilder.Entity<PhysicalClient>().HasData(
                new PhysicalClient
                {
                    IdPhysicalClient = 1,
                    Pesel = "99292006932",
                    Name = "Jan",
                    Surname = "Kowalski",
                    Email = "jan@kowalski.pl",
                    PhoneNumber = 778123432,
                    IsDeleted = false,
                },
                new PhysicalClient
                {
                    IdPhysicalClient = 2,
                    Pesel = "65292076931",
                    Name = "Piotr",
                    Surname = "Piotrkowski",
                    Email = "piotr@piotrkowski.pl",
                    PhoneNumber = 666999333,
                    IsDeleted = false,
                }
            );

            modelBuilder.Entity<Discount>().HasData(
                new Discount
                {
                    IdDiscount = 1,
                    Name = "General discount",
                    Amount = 10,
                    DateFrom = new DateTime(2023, 12, 25),
                    DateTo = new DateTime(2028, 12, 12)
                },
                new Discount
                {
                    IdDiscount = 2,
                    Name = "Birthday discount",
                    Amount = 15,
                    DateFrom = new DateTime(2020, 08, 24),
                    DateTo = new DateTime(2030, 08, 24)
                }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    IdProduct = 1,
                    Name = "Pycharm",
                    Description = "Python IDE (Pycharm) by Jetbrains",
                    VersionInfo = "1.2",
                    Category = "Software",
                    Price = 5000,
                },
                new Product
                {
                    IdProduct = 2,
                    Name = "Intellij",
                    Description = "Java IDE (Intellij) by Jetbrains",
                    VersionInfo = "15.4",
                    Category = "Software",
                    Price = 2500,
                },
                new Product
                {
                    IdProduct = 3,
                    Name = "Rider",
                    Description = "C# IDE (Rider) by Jetbrains",
                    VersionInfo = "10.9",
                    Category = "Software",
                    Price = 6500,
                }
            );

            modelBuilder.Entity<ProductDiscount>().HasData(
                new ProductDiscount
                {
                    IdProduct = 1,
                    IdDiscount = 1,
                },
                new ProductDiscount
                {
                    IdProduct = 2,
                    IdDiscount = 1,
                },
                new ProductDiscount
                {
                    IdProduct = 3,
                    IdDiscount = 2,
                }
            );

            modelBuilder.Entity<Agreement>().HasData(
                new Agreement
                {
                    IdAgreement = 1,
                    IdClient = 1,
                    ClientType = "physical",
                    IdProduct = 2,
                    ProductVersionInfo = "10.9",
                    AgreementDateFrom = new DateTime(2024, 08, 16),
                    AgreementDateTo = new DateTime(2024, 08, 30),
                    CalculatedPrice = 3400,
                    ProductUpdatesToInYears = 3,
                    IsSigned = false,
                },
                new Agreement
                {
                    IdAgreement = 2,
                    IdClient = 2,
                    ClientType = "physical",
                    IdProduct = 2,
                    ProductVersionInfo = "10.9",
                    AgreementDateFrom = new DateTime(2023, 08, 16),
                    AgreementDateTo = new DateTime(2023, 08, 30),
                    CalculatedPrice = 3400,
                    ProductUpdatesToInYears = 3,
                    IsSigned = false,
                }
            );
        }
    }
}