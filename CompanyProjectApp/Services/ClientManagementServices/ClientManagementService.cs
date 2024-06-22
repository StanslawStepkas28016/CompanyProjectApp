using CompanyProjectApp.Context;
using CompanyProjectApp.Dtos.ClientManagementDtos;
using CompanyProjectApp.Entities;
using CompanyProjectApp.Entities.ClientManagementEntities;
using Microsoft.EntityFrameworkCore;

namespace CompanyProjectApp.Services.ClientManagementServices;

public class ClientManagementService : IClientManagementService
{
    private readonly CompanyProjectAppContext _context;

    public ClientManagementService(CompanyProjectAppContext context)
    {
        _context = context;
    }

    public async Task<AddPhysicalClientDto> AddNewPhysicalClient(AddPhysicalClientDto physicalClient,
        CancellationToken cancellationToken)
    {
        if (IsPeselCorrect(physicalClient.Pesel!) == false)
        {
            throw new ArgumentException("Pesel needs to be exactly 11 digits!");
        }

        if (IsPhoneNumberCorrect(physicalClient.PhoneNumber) == false)
        {
            throw new ArgumentException("Phone number needs to be exactly 9 digits!");
        }

        if (IsEmailCorrect(physicalClient.Email!) == false)
        {
            throw new ArgumentException("Incorrect email provided!");
        }

        if (IsProvidedPhysicalClientDataCorrect(physicalClient.Name!, physicalClient.Surname!) == false)
        {
            throw new ArgumentException("Surname and/or Name are incorrect!");
        }

        if (await DoesPhysicalClientExist(physicalClient.Pesel!, cancellationToken))
        {
            throw new ArgumentException("A physical client with the provided Pesel already exists!");
        }

        var client = new PhysicalClient
        {
            Pesel = physicalClient.Pesel,
            Name = physicalClient.Name,
            Surname = physicalClient.Surname,
            Email = physicalClient.Email,
            PhoneNumber = physicalClient.PhoneNumber,
            IsDeleted = false
        };

        await _context
            .PhysicalClients
            .AddAsync(client, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return physicalClient;
    }

    public async Task<AddPhysicalClientDto> ModifyPhysicalClient(string physicalClientPesel,
        ModifyPhysicalClientDto physicalClientDto,
        CancellationToken cancellationToken)
    {
        if (IsPeselCorrect(physicalClientPesel) == false)
        {
            throw new ArgumentException("Pesel needs to be exactly 11 digits!");
        }

        if (IsEmailCorrect(physicalClientDto.Email!) == false)
        {
            throw new ArgumentException("Incorrect email provided!");
        }

        if (IsPhoneNumberCorrect(physicalClientDto.PhoneNumber) == false)
        {
            throw new ArgumentException("Phone number needs to be exactly 9 digits!");
        }

        if (IsProvidedPhysicalClientDataCorrect(physicalClientDto.Name!, physicalClientDto.Surname!) == false)
        {
            throw new ArgumentException("Surname and/or Name are incorrect!");
        }

        if (await DoesPhysicalClientExist(physicalClientPesel, cancellationToken) == false)
        {
            throw new ArgumentException("A client with the provided Pesel does not exist!");
        }

        var physicalClient = await _context
            .PhysicalClients
            .Where(pc => pc.Pesel == physicalClientPesel)
            .FirstOrDefaultAsync(cancellationToken);

        physicalClient!.Name = physicalClientDto.Name;
        physicalClient.Surname = physicalClientDto.Surname;
        physicalClient.Email = physicalClientDto.Email;
        physicalClient.PhoneNumber = physicalClientDto.PhoneNumber;

        await _context.SaveChangesAsync(cancellationToken);

        return new AddPhysicalClientDto
        {
            Pesel = physicalClientPesel,
            Name = physicalClient.Name,
            Surname = physicalClient.Surname,
            Email = physicalClient.Email,
            PhoneNumber = physicalClient.PhoneNumber
        };
    }

    public async Task<int> DeletePhysicalClient(string physicalClientPesel, CancellationToken cancellationToken)
    {
        if (IsPeselCorrect(physicalClientPesel) == false)
        {
            throw new ArgumentException("Pesel needs to be exactly 11 digits!");
        }

        if (await DoesPhysicalClientExist(physicalClientPesel, cancellationToken) == false)
        {
            throw new ArgumentException("A physical client with the provided Pesel does not exist!");
        }

        var physicalClient = await _context
            .PhysicalClients
            .Where(pc => pc.Pesel == physicalClientPesel)
            .FirstOrDefaultAsync(cancellationToken);

        physicalClient!.IsDeleted = true;

        await _context.SaveChangesAsync(cancellationToken);

        return 1;
    }

    public async Task<AddCompanyClientDto> AddNewCompanyClient(AddCompanyClientDto companyClientDto,
        CancellationToken cancellationToken)
    {
        if (IsKrsCorrect(companyClientDto.KrsNumber!) == false)
        {
            throw new ArgumentException("Krs number has to be either 9 or 14 digits!");
        }

        if (IsEmailCorrect(companyClientDto.Email!) == false)
        {
            throw new ArgumentException("Provided email is incorrect!");
        }

        if (IsPhoneNumberCorrect(companyClientDto.PhoneNumber) == false)
        {
            throw new ArgumentException("Provided phone number is incorrect!");
        }

        if (IsAddressCorrect(companyClientDto.Address!) == false)
        {
            throw new AggregateException("Provided address is incorrect!");
        }

        if (await DoesCompanyClientExist(companyClientDto.KrsNumber!, cancellationToken))
        {
            throw new AggregateException("A company client with the provided Krs number already exists!");
        }

        var client = new CompanyClient()
        {
            KrsNumber = companyClientDto.KrsNumber,
            Email = companyClientDto.Email,
            Address = companyClientDto.Address,
            PhoneNumber = companyClientDto.PhoneNumber
        };

        await _context
            .CompanyClients
            .AddAsync(client, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return companyClientDto;
    }

    public async Task<AddCompanyClientDto> ModifyCompanyClient(string krsNumber,
        ModifyCompanyClientDto companyClientDto,
        CancellationToken cancellationToken)
    {
        if (IsKrsCorrect(krsNumber) == false)
        {
            throw new ArgumentException("Krs number has to be either 9 or 14 digits!");
        }

        if (IsEmailCorrect(companyClientDto.Email!) == false)
        {
            throw new ArgumentException("Provided email is incorrect!");
        }

        if (IsPhoneNumberCorrect(companyClientDto.PhoneNumber) == false)
        {
            throw new ArgumentException("Provided phone number is incorrect!");
        }

        if (IsAddressCorrect(companyClientDto.Address!) == false)
        {
            throw new AggregateException("Provided address is incorrect!");
        }

        if (await DoesCompanyClientExist(krsNumber, cancellationToken) == false)
        {
            throw new AggregateException("A company client with the provided Krs number does not exist!");
        }

        var companyClient = await _context
            .CompanyClients
            .Where(cc => cc.KrsNumber == krsNumber)
            .FirstOrDefaultAsync(cancellationToken);

        companyClient!.Email = companyClientDto.Email;
        companyClient.Address = companyClientDto.Address;
        companyClient.PhoneNumber = companyClientDto.PhoneNumber;

        await _context.SaveChangesAsync(cancellationToken);

        return new AddCompanyClientDto
        {
            KrsNumber = krsNumber,
            Address = companyClient.Address,
            Email = companyClient.Email,
            PhoneNumber = companyClient.PhoneNumber
        };
    }

    private async Task<bool> DoesPhysicalClientExist(string pesel,
        CancellationToken cancellationToken)
    {
        var res = await _context
            .PhysicalClients
            .Where(pc => pc.Pesel == pesel)
            .FirstOrDefaultAsync(cancellationToken);

        return res != null;
    }

    private async Task<bool> DoesCompanyClientExist(string krsNumber,
        CancellationToken cancellationToken)
    {
        var res = await _context
            .CompanyClients
            .Where(pc => pc.KrsNumber == krsNumber)
            .FirstOrDefaultAsync(cancellationToken);

        return res != null;
    }

    private bool IsPeselCorrect(string pesel)
    {
        return pesel.Length == 11 && long.TryParse(pesel, out _);
    }

    private bool IsPhoneNumberCorrect(int phoneNumber)
    {
        return phoneNumber.ToString().Length == 9;
    }

    private bool IsEmailCorrect(string email)
    {
        return email.Contains('@') && email.Contains('.');
    }

    private bool IsProvidedPhysicalClientDataCorrect(string name, string surname)
    {
        return !string.IsNullOrWhiteSpace(name) && !string.IsNullOrWhiteSpace(surname)
                                                && name != "string" && name != "string";
    }

    private bool IsKrsCorrect(string krsNumber)
    {
        return krsNumber.Length is 9 or 14 && !string.IsNullOrWhiteSpace(krsNumber) && long.TryParse(krsNumber, out _);
    }

    private bool IsAddressCorrect(string address)
    {
        return !string.IsNullOrWhiteSpace(address) && address != "string";
    }
}