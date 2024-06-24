using CompanyProjectApp.Context;
using CompanyProjectApp.Entities.ProductEntities;
using CompanyProjectAppTests.Setup;
using Microsoft.EntityFrameworkCore;
using Assert = NUnit.Framework.Assert;

namespace CompanyProjectAppTests.RevenueService;

public class RevenueServiceTests
{
    private readonly CompanyProjectAppContext _context;
    private readonly CompanyProjectApp.Services.RevenueServices.RevenueService _service;

    public RevenueServiceTests()
    {
        _context = CompanyProjectAppDbContextForTestsFactory.CreateDbContextForInMemory();
        _service = new CompanyProjectApp.Services.RevenueServices.RevenueService(_context);
    }

    [Fact]
    public async Task CalculateActualRevenueForTheWholeCompanyShouldCalculateCorrectRevenue()
    {
        var agreementSigned = new Agreement
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
            IsSigned = true,
        };

        await _context.Agreements.AddAsync(agreementSigned);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 2,
            IdClient = 2,
            ClientType = "physical",
            IdProduct = 1,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 08, 16),
            AgreementDateTo = new DateTime(2024, 08, 30),
            CalculatedPrice = 5000,
            ProductUpdatesToInYears = 3,
            IsSigned = false,
        });

        await _context.Payments.AddAsync(new Payment
        {
            IdPayment = 1,
            IdAgreement = 1,
            MoneyOwedFull = 3400,
            MoneyPaid = 3400,
        });

        await _context.SaveChangesAsync();

        var revenue = await _service.CalculateActualRevenueForTheWholeCompany("USD", new CancellationToken());
        var apiRate = await _service.GetExchangeRate("USD", new CancellationToken());
        var compareRevenue = apiRate * (double)agreementSigned.CalculatedPrice;

        Assert.That(revenue,
            Is.EqualTo(compareRevenue));
    }

    [Fact]
    public async Task CalculateActualRevenueForTheWholeCompanyShouldBeZeroWhenNoAgreementIsSigned()
    {
        var agreementSigned = new Agreement
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
        };

        await _context.Agreements.AddAsync(agreementSigned);

        await _context.Payments.AddAsync(new Payment
        {
            IdPayment = 1,
            IdAgreement = 1,
            MoneyOwedFull = 3400,
            MoneyPaid = 300,
        });

        await _context.SaveChangesAsync();

        var revenue = await _service.CalculateActualRevenueForTheWholeCompany("USD", new CancellationToken());

        Assert.That(revenue,
            Is.EqualTo(0));
    }

    [Fact]
    public async Task CalculateExpectedRevenueForTheWholeCompanyShouldReturnTheExpectedRevenue()
    {
        await _context.Agreements.AddAsync(new Agreement
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
        });

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 2,
            IdClient = 2,
            ClientType = "physical",
            IdProduct = 1,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 08, 16),
            AgreementDateTo = new DateTime(2024, 08, 30),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = true,
        });

        await _context.SaveChangesAsync();

        var revenue = await _service.CalculateExpectedRevenueForTheWholeCompany("USD", new CancellationToken());
        var apiRate = await _service.GetExchangeRate("USD", new CancellationToken());
        var expectedRevenue = await _context.Agreements.SumAsync(a => a.CalculatedPrice);

        Assert.That(revenue,
            Is.EqualTo(apiRate * (double)expectedRevenue));
    }

    [Fact]
    public async Task CalculateActualRevenueForAProductShouldReturnTheActualRevenueForAProduct()
    {
        var product = new Product
        {
            IdProduct = 3,
            Name = "Rider",
            Description = "C# IDE (Rider) by Jetbrains",
            VersionInfo = "10.9",
            Category = "Software",
            Price = 2000,
        };

        await _context.Products.AddAsync(product);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 1,
            IdClient = 2,
            ClientType = "physical",
            IdProduct = 3,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 08, 16),
            AgreementDateTo = new DateTime(2024, 08, 30),
            CalculatedPrice = 5000,
            ProductUpdatesToInYears = 3,
            IsSigned = false
        });

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 2,
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 3,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 08, 16),
            AgreementDateTo = new DateTime(2024, 08, 30),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = true
        });

        await _context.SaveChangesAsync();

        var productRevenue = await _service.CalculateActualRevenueForAProduct(3, "USD", new CancellationToken());

        var apiRate = await _service.GetExchangeRate("USD", new CancellationToken());
        var productPrice = await _context.Agreements.Where(a => a.IdProduct == product.IdProduct && a.IsSigned)
            .SumAsync(a => a.CalculatedPrice);

        Assert.That(productRevenue, Is.EqualTo(apiRate * (double)productPrice));
    }

    [Fact]
    public Task CalculateActualRevenueForAProductShouldThrowExceptionWhenProductDoesNotExist()
    {
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CalculateActualRevenueForAProduct(3, "USD", new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("A product with the provided Id does not exist!"));
        return Task.CompletedTask;
    }
}