using System.Globalization;
using CompanyProjectApp.Dtos.ProductClientDtos;
using CompanyProjectApp.Entities;
using CompanyProjectApp.Entities.ClientManagementEntities;
using CompanyProjectApp.Entities.ProductEntities;
using CompanyProjectAppTests.Setup;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;
using Assert = NUnit.Framework.Assert;

namespace CompanyProjectAppTests.AgreementService;

public class AgreementServiceTests
{
    private readonly ITestOutputHelper _testOutputHelper;
    private readonly CompanyProjectAppContext _context;
    private readonly CompanyProjectApp.Services.AgreementServices.AgreementService _service;

    public AgreementServiceTests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
        _context = CompanyProjectAppDbContextForTestsFactory.CreateDbContextForInMemory();
        _service = new CompanyProjectApp.Services.AgreementServices.AgreementService(_context);
    }

    [Fact]
    public async Task CreateAgreementShouldInFactAddAnAgreementToTheDatabase()
    {
        await _context.Discounts.AddAsync(
            new Discount
            {
                IdDiscount = 1,
                Name = "General discount",
                Amount = 10,
                DateFrom = new DateTime(2023, 12, 25),
                DateTo = new DateTime(2028, 12, 12)
            }
        );

        await _context.Products.AddAsync(new Product
        {
            IdProduct = 1,
            Name = "Pycharm",
            Description = "Python IDE (Pycharm) by Jetbrains",
            VersionInfo = "1.2",
            Category = "Software",
            Price = 5000,
            IdDiscount = 1,
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        await _context.PhysicalClients.AddAsync(new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        });

        await _context.SaveChangesAsync();

        var createAgreementResponseDto = await _service.CreateAgreement(new CreateAgreementRequestDto
        {
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 1,
            AgreementDateFrom = new DateTime(2023, 08, 16),
            AgreementDateTo = new DateTime(2023, 08, 30),
            ProductUpdatesToInYears = 3,
        }, new CancellationToken());

        var count = await _context.Agreements.CountAsync();

        _testOutputHelper.WriteLine(createAgreementResponseDto.CalculatedPrice.ToString(CultureInfo.InvariantCulture));
        Assert.That(count, Is.EqualTo(1));
    }
}