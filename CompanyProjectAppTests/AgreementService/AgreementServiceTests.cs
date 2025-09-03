using CompanyProjectApp.Context;
using CompanyProjectApp.Dtos.AgreementDtos;
using CompanyProjectApp.Dtos.ProductClientDtos;
using CompanyProjectApp.Entities.ClientManagementEntities;
using CompanyProjectApp.Entities.ProductEntities;
using CompanyProjectAppTests.Setup;
using Microsoft.EntityFrameworkCore;
using Assert = NUnit.Framework.Assert;

namespace CompanyProjectAppTests.AgreementService;

public class AgreementServiceTests
{
    private readonly CompanyProjectAppContext _context;
    private readonly CompanyProjectApp.Services.AgreementServices.AgreementService _service;

    public AgreementServiceTests()
    {
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

        var agreementResponse = await _service.CreateAgreement(new CreateAgreementRequestDto
        {
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 1,
            AgreementDateFrom = new DateTime(2023, 08, 16),
            AgreementDateTo = new DateTime(2023, 08, 30),
            ProductUpdatesToInYears = 3,
        }, new CancellationToken());

        var count = await _context.Agreements.CountAsync();

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(agreementResponse.CalculatedPrice, Is.EqualTo(7500));
            Assert.That(agreementResponse.IsSigned, Is.EqualTo(false));
        });
    }

    [Fact]
    public async Task CreateAgreementShouldThrowExceptionWhenDateFromAndDateToAreIncorrect()
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

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreateAgreement(new CreateAgreementRequestDto
            {
                IdClient = 1,
                ClientType = "physical",
                IdProduct = 1,
                AgreementDateFrom = new DateTime(2023, 08, 16),
                AgreementDateTo = new DateTime(2025, 08, 30),
                ProductUpdatesToInYears = 3,
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Agreement date has to be between 3 and 30 days!"));
    }

    [Fact]
    public async Task CreateAgreementShouldThrowExceptionWhenClientTypeIsIncorrect()
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

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreateAgreement(new CreateAgreementRequestDto
            {
                IdClient = 1,
                ClientType = "whatever",
                IdProduct = 1,
                AgreementDateFrom = new DateTime(2023, 08, 16),
                AgreementDateTo = new DateTime(2023, 08, 30),
                ProductUpdatesToInYears = 3,
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("ClientType has to be either \"company\" or \"physical\"!"));
    }

    [Fact]
    public async Task CreateAgreementShouldThrowExceptionWhenUpdateYearsAreIncorrect()
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

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreateAgreement(new CreateAgreementRequestDto
            {
                IdClient = 1,
                ClientType = "physical",
                IdProduct = 1,
                AgreementDateFrom = new DateTime(2023, 08, 16),
                AgreementDateTo = new DateTime(2023, 08, 30),
                ProductUpdatesToInYears = 6,
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("ProductUpdatesToInYears can be between 1 and 3 years!"));
    }

    [Fact]
    public async Task CreateAgreementShouldThrowExceptionWhenClientDoesNotExist()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        await _context.SaveChangesAsync();

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreateAgreement(new CreateAgreementRequestDto
            {
                IdClient = 1,
                ClientType = "physical",
                IdProduct = 1,
                AgreementDateFrom = new DateTime(2023, 08, 16),
                AgreementDateTo = new DateTime(2023, 08, 30),
                ProductUpdatesToInYears = 2,
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Client with the provided IdClient does not exist!"));
    }

    [Fact]
    public async Task CreateAgreementShouldThrowExceptionWhenProductDoesNotExist()
    {
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

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreateAgreement(new CreateAgreementRequestDto
            {
                IdClient = 1,
                ClientType = "physical",
                IdProduct = 1,
                AgreementDateFrom = new DateTime(2023, 08, 16),
                AgreementDateTo = new DateTime(2023, 08, 30),
                ProductUpdatesToInYears = 2,
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Product with the provided IdProduct does not exist!"));
    }

    [Fact]
    public async Task CreateAgreementShouldThrowExceptionWhenClientAlreadyHasAgreementForProduct()
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

        await _service.CreateAgreement(new CreateAgreementRequestDto
        {
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 1,
            AgreementDateFrom = new DateTime(2023, 08, 16),
            AgreementDateTo = new DateTime(2023, 08, 30),
            ProductUpdatesToInYears = 3,
        }, new CancellationToken());

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.CreateAgreement(new CreateAgreementRequestDto
            {
                IdClient = 1,
                ClientType = "physical",
                IdProduct = 1,
                AgreementDateFrom = new DateTime(2023, 08, 16),
                AgreementDateTo = new DateTime(2023, 08, 30),
                ProductUpdatesToInYears = 3,
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("Provided client already has a yet unsigned agreement for the specified product!"));
    }

    [Fact]
    public async Task PayForAgreementShouldAddPaymentForTheFirstPayment()
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

        var agreementResponse = await _service.CreateAgreement(new CreateAgreementRequestDto
        {
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 1,
            AgreementDateFrom = new DateTime(2024, 06, 23),
            AgreementDateTo = new DateTime(2024, 07, 12),
            ProductUpdatesToInYears = 3,
        }, new CancellationToken());

        var paymentResponse = await _service.PayForAgreement(new PayForAgreementRequestDto
        {
            IdAgreement = agreementResponse.IdAgreement,
            IdClient = agreementResponse.IdClient,
            ClientType = agreementResponse.ClientType,
            Amount = 500,
        }, new CancellationToken());

        var count = await _context.Payments.CountAsync();

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(paymentResponse.IdAgreement, Is.EqualTo(agreementResponse.IdAgreement));
            Assert.That(paymentResponse.MoneyPaid, Is.EqualTo(500));
            Assert.That(paymentResponse.MoneyOwedFull, Is.EqualTo(7500));
        });
    }

    [Fact]
    public async Task PayForAgreementShouldAddIncreaseMoneyPaidWhenPayingInFractions()
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

        var agreementResponse = await _service.CreateAgreement(new CreateAgreementRequestDto
        {
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 1,
            AgreementDateFrom = new DateTime(2024, 06, 23),
            AgreementDateTo = new DateTime(2024, 07, 12),
            ProductUpdatesToInYears = 3,
        }, new CancellationToken());

        var firstAmount = 500;
        var secondAmount = 2000;

        await _service.PayForAgreement(new PayForAgreementRequestDto
        {
            IdAgreement = agreementResponse.IdAgreement,
            IdClient = agreementResponse.IdClient,
            ClientType = agreementResponse.ClientType,
            Amount = firstAmount,
        }, new CancellationToken());

        var secondPaymentResponse = await _service.PayForAgreement(new PayForAgreementRequestDto
        {
            IdAgreement = agreementResponse.IdAgreement,
            IdClient = agreementResponse.IdClient,
            ClientType = agreementResponse.ClientType,
            Amount = secondAmount,
        }, new CancellationToken());

        var count = await _context.Payments.CountAsync();

        Assert.Multiple(() =>
        {
            Assert.That(count, Is.EqualTo(1));
            Assert.That(secondPaymentResponse.IdAgreement, Is.EqualTo(agreementResponse.IdAgreement));
            Assert.That(secondPaymentResponse.MoneyPaid, Is.EqualTo(firstAmount + secondAmount));
            Assert.That(secondPaymentResponse.MoneyOwedFull, Is.EqualTo(7500));
        });
    }

    [Fact]
    public async Task PayForAgreementShouldThrowExceptionWhenNoAgreementForClientAndProductIsFound()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        var client = new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        };

        await _context.PhysicalClients.AddAsync(client);

        await _context.SaveChangesAsync();

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.PayForAgreement(new PayForAgreementRequestDto
            {
                IdAgreement = 1,
                IdClient = client.IdPhysicalClient,
                ClientType = "physical",
                Amount = 200,
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("Agreement with the provided IdAgreement, IdClient and ClientType does not exist!"));
    }

    [Fact]
    public async Task PayForAgreementShouldThrowExceptionWhenAgreementIsAlreadyPaidMeaningSigned()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        var client = new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        };

        await _context.PhysicalClients.AddAsync(client);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 1,
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 2,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 06, 23),
            AgreementDateTo = new DateTime(2024, 07, 12),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = true,
        });

        await _context.SaveChangesAsync();

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.PayForAgreement(new PayForAgreementRequestDto
            {
                IdAgreement = 1,
                IdClient = client.IdPhysicalClient,
                ClientType = "physical",
                Amount = 200,
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("The provided agreement has been already paid for (it is already signed)!"));
    }

    [Fact]
    public async Task PayForAgreementShouldThrowExceptionWhenClientIsOverpayingWhenPayingFullPrice()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        var client = new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        };

        await _context.PhysicalClients.AddAsync(client);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 1,
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 2,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 06, 23),
            AgreementDateTo = new DateTime(2024, 07, 12),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = false,
        });

        await _context.SaveChangesAsync();

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.PayForAgreement(new PayForAgreementRequestDto
            {
                IdAgreement = 1,
                IdClient = client.IdPhysicalClient,
                ClientType = "physical",
                Amount = 3800,
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("You cannot overpay for your agreement (you are trying to pay more than full price)!"));
    }

    [Fact]
    public async Task PayForAgreementShouldThrowExceptionWhenAgreementHasNotBegun()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        var client = new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        };

        await _context.PhysicalClients.AddAsync(client);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 1,
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 2,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2025, 06, 23),
            AgreementDateTo = new DateTime(2025, 07, 12),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = false,
        });

        await _context.SaveChangesAsync();

        var exception = Assert.ThrowsAsync<Exception>(async () =>
        {
            await _service.PayForAgreement(new PayForAgreementRequestDto
            {
                IdAgreement = 1,
                IdClient = client.IdPhysicalClient,
                ClientType = "physical",
                Amount = 200,
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("The agreement has not begun, you cannot pay for it!"));
    }

    [Fact]
    public async Task PayForAgreementShouldThrowExceptionAndRemoveAgreementWithPaymentsWhenClientIsLateWithPayment()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        var client = new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        };

        await _context.PhysicalClients.AddAsync(client);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 1,
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 2,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2023, 06, 23),
            AgreementDateTo = new DateTime(2023, 07, 12),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = false,
        });

        await _context.SaveChangesAsync();

        var agreementsCountBeforeLatePayment = await _context.Agreements.CountAsync();

        var exception = Assert.ThrowsAsync<Exception>(async () =>
        {
            await _service.PayForAgreement(new PayForAgreementRequestDto
            {
                IdAgreement = 1,
                IdClient = client.IdPhysicalClient,
                ClientType = "physical",
                Amount = 200,
            }, new CancellationToken());
        });

        var agreementsCountAfterLatePayment = await _context.Agreements.CountAsync();
        var paymentsCountAfterLatePayment = await _context.Payments.CountAsync();

        Assert.Multiple(() =>
        {
            Assert.That(exception.Message,
                Is.EqualTo(
                    "You are late with your payment! The money you have paid, will be returned and the agreement will be dismissed!"));
            Assert.That(agreementsCountBeforeLatePayment, Is.EqualTo(1));
            Assert.That(agreementsCountAfterLatePayment, Is.EqualTo(0));
            Assert.That(paymentsCountAfterLatePayment, Is.EqualTo(0));
        });
    }

    [Fact]
    public async Task PayForAgreementShouldThrowExceptionWhenClientIsOverpayingWhenPayingInFractions()
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
        });

        await _context.ProductDiscounts.AddAsync(new ProductDiscount
        {
            IdProduct = 1,
            IdDiscount = 1,
        });

        var client = new PhysicalClient
        {
            IdPhysicalClient = 1,
            Pesel = "99292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
            IsDeleted = false,
        };

        await _context.PhysicalClients.AddAsync(client);

        await _context.Agreements.AddAsync(new Agreement
        {
            IdAgreement = 1,
            IdClient = 1,
            ClientType = "physical",
            IdProduct = 2,
            ProductVersionInfo = "10.9",
            AgreementDateFrom = new DateTime(2024, 06, 23),
            AgreementDateTo = new DateTime(2024, 07, 12),
            CalculatedPrice = 3400,
            ProductUpdatesToInYears = 3,
            IsSigned = false,
        });

        await _context.SaveChangesAsync();

        await _service.PayForAgreement(new PayForAgreementRequestDto
        {
            IdAgreement = 1,
            IdClient = client.IdPhysicalClient,
            ClientType = "physical",
            Amount = 400,
        }, new CancellationToken());

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.PayForAgreement(new PayForAgreementRequestDto
            {
                IdAgreement = 1,
                IdClient = client.IdPhysicalClient,
                ClientType = "physical",
                Amount = 3100,
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("You cannot overpay for your agreement (while paying in fractions)!"));
    }
}