using CompanyProjectApp.Context;
using CompanyProjectApp.Dtos.ClientManagementDtos;
using CompanyProjectApp.Entities;
using CompanyProjectAppTests.Setup;
using Microsoft.EntityFrameworkCore;
using Assert = NUnit.Framework.Assert;

namespace CompanyProjectAppTests.ClientManagementService;

public class ClientManagementServiceTests
{
    private readonly CompanyProjectAppContext _context;
    private readonly CompanyProjectApp.Services.ClientManagementServices.ClientManagementService _service;

    public ClientManagementServiceTests()
    {
        _context = CompanyProjectAppDbContextForTestsFactory.CreateDbContextForInMemory();
        _service = new CompanyProjectApp.Services.ClientManagementServices.ClientManagementService(_context);
    }

    [Fact]
    public async Task AddPhysicalUserShouldInFactAddOneUserToTheDatabase()
    {
        var physicalClientDto = new AddPhysicalClientDto()
        {
            Pesel = "03292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
        };

        await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());

        var res = await _context.PhysicalClients.CountAsync();
        Assert.That(res, Is.EqualTo(1));
    }

    [Fact]
    public Task AddPhysicalUserShouldThrowExceptionForInvalidPesel()
    {
        var physicalClientDto = new AddPhysicalClientDto()
        {
            Pesel = "9929",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 778123432,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Pesel needs to be exactly 11 digits!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task AddPhysicalUserShouldThrowExceptionForInvalidPhoneNumber()
    {
        var physicalClientDto = new AddPhysicalClientDto()
        {
            Pesel = "03292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jan@kowalski.pl",
            PhoneNumber = 77812,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Phone number needs to be exactly 9 digits!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task AddPhysicalUserShouldThrowExceptionForInvalidEmail()
    {
        var physicalClientDto = new AddPhysicalClientDto()
        {
            Pesel = "03292006932",
            Name = "Jan",
            Surname = "Kowalski",
            Email = "jankowalskipl",
            PhoneNumber = 798666231,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Incorrect email provided!"));
        return Task.CompletedTask;
    }


    [Fact]
    public Task AddPhysicalUserShouldThrowExceptionForIncorrectClientNameAndSurname()
    {
        var physicalClientDto = new AddPhysicalClientDto()
        {
            Pesel = "03292006932",
            Name = "string",
            Surname = null,
            Email = "jan@kowalski.pl",
            PhoneNumber = 798666231,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Surname and/or Name are incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddPhysicalUserShouldThrowExceptionWhenClientWithTheSpecifiedPeselAlreadyExists()
    {
        var physicalClientDto = new AddPhysicalClientDto()
        {
            Pesel = "03292006932",
            Name = "Robert",
            Surname = "Kowalski",
            Email = "rob@kowalski.pl",
            PhoneNumber = 798666231,
        };

        await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewPhysicalClient(physicalClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("A physical client with the provided Pesel already exists!"));
    }

    [Fact]
    public async Task ModifyPhysicalClientShouldInFactModifyUsersData()
    {
        var addPhysicalClientDto = new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        };
        await _service.AddNewPhysicalClient(addPhysicalClientDto, new CancellationToken());

        var pesel = "03292006932";

        var modifyCompanyClientDto = new ModifyPhysicalClientDto
        {
            Email = "rob@kowalski.pl",
            Name = "Robert",
            Surname = "Kowalski",
            PhoneNumber = 798666231,
        };

        await _service.ModifyPhysicalClient(pesel, modifyCompanyClientDto, new CancellationToken());

        var modifiedClient = await _context.PhysicalClients.Where(pc => pc.Pesel == pesel).FirstOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.That(modifiedClient!.Email, Is.EqualTo(modifyCompanyClientDto.Email));
            Assert.That(modifiedClient.Name, Is.EqualTo(modifyCompanyClientDto.Name));
            Assert.That(modifiedClient.Surname, Is.EqualTo(modifyCompanyClientDto.Surname));
            Assert.That(modifiedClient.PhoneNumber, Is.EqualTo(modifyCompanyClientDto.PhoneNumber));
        });
    }

    [Fact]
    public async Task ModifyPhysicalClientShouldThrowExceptionWhenPeselIsIncorrect()
    {
        var addPhysicalClientDto = new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        };
        await _service.AddNewPhysicalClient(addPhysicalClientDto, new CancellationToken());

        var pesel = "0321123";

        var modifyCompanyClientDto = new ModifyPhysicalClientDto
        {
            Email = "rob@kowalski.pl",
            Name = "Robert",
            Surname = "Kowalski",
            PhoneNumber = 798666231,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyPhysicalClient(pesel, modifyCompanyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Pesel needs to be exactly 11 digits!"));
    }


    [Fact]
    public async Task ModifyPhysicalClientShouldThrowExceptionWhenEmailIsIncorrect()
    {
        var addPhysicalClientDto = new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        };
        await _service.AddNewPhysicalClient(addPhysicalClientDto, new CancellationToken());

        var pesel = "03292006932";

        var modifyCompanyClientDto = new ModifyPhysicalClientDto
        {
            Email = "robkowalski.pl",
            Name = "Robert",
            Surname = "Kowalski",
            PhoneNumber = 798666231,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyPhysicalClient(pesel, modifyCompanyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Incorrect email provided!"));
    }

    [Fact]
    public async Task ModifyPhysicalClientShouldThrowExceptionWhenPhoneNumberIsIncorrect()
    {
        var addPhysicalClientDto = new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        };
        await _service.AddNewPhysicalClient(addPhysicalClientDto, new CancellationToken());

        var pesel = "03292006932";

        var modifyCompanyClientDto = new ModifyPhysicalClientDto
        {
            Email = "rob@kowalski.pl",
            Name = "Robert",
            Surname = "Kowalski",
            PhoneNumber = 12312,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyPhysicalClient(pesel, modifyCompanyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Phone number needs to be exactly 9 digits!"));
    }

    [Fact]
    public async Task ModifyPhysicalClientShouldThrowExceptionForIncorrectClientNameAndSurname()
    {
        var addPhysicalClientDto = new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        };
        await _service.AddNewPhysicalClient(addPhysicalClientDto, new CancellationToken());

        var modifyCompanyClientDto = new ModifyPhysicalClientDto
        {
            Email = "rob@kowalski.pl",
            Name = "string",
            Surname = null,
            PhoneNumber = 696784867,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyPhysicalClient(addPhysicalClientDto.Pesel, modifyCompanyClientDto,
                new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Surname and/or Name are incorrect!"));
    }

    [Fact]
    public async Task ModifyPhysicalClientShouldThrowExceptionWhenClientWithTheProvidedPeselDoesNotExist()
    {
        var addPhysicalClientDto = new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        };
        await _service.AddNewPhysicalClient(addPhysicalClientDto, new CancellationToken());

        var modifyCompanyClientDto = new ModifyPhysicalClientDto
        {
            Email = "rob@doe.pl",
            Name = "Robert",
            Surname = "Doe",
            PhoneNumber = 696784867,
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyPhysicalClient("03202006932", modifyCompanyClientDto,
                new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("A client with the provided Pesel does not exist!"));
    }

    [Fact]
    public async Task DeletePhysicalClientShouldInFactShallowDeletePhysicalClientFromDatabase()
    {
        await _service.AddNewPhysicalClient(new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        }, new CancellationToken());

        await _service.DeletePhysicalClient("03292006932", new CancellationToken());

        var physicalClient =
            await _context.PhysicalClients.Where(pc => pc.Pesel == "03292006932").FirstOrDefaultAsync();

        Assert.That(physicalClient!.IsDeleted, Is.EqualTo(true));
    }

    [Fact]
    public async Task DeletePhysicalClientShouldThrowExceptionWhenClientPeselIsIncorrect()
    {
        await _service.AddNewPhysicalClient(new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        }, new CancellationToken());


        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.DeletePhysicalClient("asdasdas", new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Pesel needs to be exactly 11 digits!"));
    }

    [Fact]
    public async Task DeletePhysicalClientShouldThrowExceptionWhenClientWithSpecifiedPeselDoesNotExist()
    {
        await _service.AddNewPhysicalClient(new AddPhysicalClientDto
        {
            Pesel = "03292006932",
            Email = "john@doe.pl",
            Name = "John",
            Surname = "Doe",
            PhoneNumber = 666444999,
        }, new CancellationToken());


        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.DeletePhysicalClient("93292006932", new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("A physical client with the provided Pesel does not exist!"));
    }

    [Fact]
    public async Task AddCompanyClientShouldAddCompanyClientToDatabase()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };

        await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());

        var res = await _context.CompanyClients.CountAsync();
        Assert.That(res, Is.EqualTo(1));
    }

    [Fact]
    public Task AddCompanyClientShouldThrowExceptionForInvalidKrsNumber()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Krs number has to be either 9 or 14 digits!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task AddCompanyClientShouldThrowExceptionForInvalidEmail()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john.doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Provided email is incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task AddCompanyClientShouldThrowExceptionForInvalidPhoneNumber()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 123
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Provided phone number is incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task AddCompanyClientShouldThrowExceptionForInvalidAddress()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "string",
            PhoneNumber = 666444999
        };

        var exception = Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Provided address is incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public async Task AddCompanyClientShouldThrowExceptionWhenClientWithTheSpecifiedKrsNumberAlreadyExists()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa, Warszawa",
            PhoneNumber = 666444999
        };

        await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());

        var exception = Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("A company client with the provided Krs number already exists!"));
    }

    [Fact]
    public async Task ModifyCompanyClientShouldModifyCompanyClientData()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };
        await _service.AddNewCompanyClient(companyClientDto, new CancellationToken());

        var modifyCompanyClientDto = new ModifyCompanyClientDto
        {
            Email = "jane@doe.pl",
            Address = "456 Elm St",
            PhoneNumber = 777555333
        };

        await _service.ModifyCompanyClient(companyClientDto.KrsNumber, modifyCompanyClientDto, new CancellationToken());

        var modifiedClient = await _context.CompanyClients.Where(cc => cc.KrsNumber == companyClientDto.KrsNumber)
            .FirstOrDefaultAsync();

        Assert.Multiple(() =>
        {
            Assert.That(modifiedClient!.Email, Is.EqualTo(modifyCompanyClientDto.Email));
            Assert.That(modifiedClient.Address, Is.EqualTo(modifyCompanyClientDto.Address));
            Assert.That(modifiedClient.PhoneNumber, Is.EqualTo(modifyCompanyClientDto.PhoneNumber));
        });
    }

    [Fact]
    public Task ModifyCompanyClientShouldThrowExceptionForInvalidKrsNumber()
    {
        var modifyCompanyClientDto = new ModifyCompanyClientDto
        {
            Email = "jane@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 777555333
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyCompanyClient("123", modifyCompanyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Krs number has to be either 9 or 14 digits!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task ModifyCompanyClientShouldThrowExceptionForInvalidEmail()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };

        var modifyCompanyClientDto = new ModifyCompanyClientDto
        {
            Email = "jane.doe.pl",
            Address = "Elsnera 34, Poznań",
            PhoneNumber = 777555333
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyCompanyClient(companyClientDto.KrsNumber, modifyCompanyClientDto,
                new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Provided email is incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task ModifyCompanyClientShouldThrowExceptionForInvalidPhoneNumber()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };

        var modifyCompanyClientDto = new ModifyCompanyClientDto
        {
            Email = "jane@doe.pl",
            Address = "Elsnera 34, Poznań",
            PhoneNumber = 123
        };

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.ModifyCompanyClient(companyClientDto.KrsNumber, modifyCompanyClientDto,
                new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Provided phone number is incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task ModifyCompanyClientShouldThrowExceptionForInvalidAddress()
    {
        var companyClientDto = new AddCompanyClientDto
        {
            KrsNumber = "123456789",
            Email = "john@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 666444999
        };

        var modifyCompanyClientDto = new ModifyCompanyClientDto
        {
            Email = "jane@doe.pl",
            Address = "string",
            PhoneNumber = 777555333
        };

        var exception = Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await _service.ModifyCompanyClient(companyClientDto.KrsNumber, modifyCompanyClientDto,
                new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Provided address is incorrect!"));
        return Task.CompletedTask;
    }

    [Fact]
    public Task ModifyCompanyClientShouldThrowExceptionWhenClientWithTheProvidedKrsNumberDoesNotExist()
    {
        var modifyCompanyClientDto = new ModifyCompanyClientDto
        {
            Email = "jane@doe.pl",
            Address = "Kwiatowa 13, Warszawa",
            PhoneNumber = 777555333
        };

        var exception = Assert.ThrowsAsync<AggregateException>(async () =>
        {
            await _service.ModifyCompanyClient("987654321", modifyCompanyClientDto, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("A company client with the provided Krs number does not exist!"));
        return Task.CompletedTask;
    }
}