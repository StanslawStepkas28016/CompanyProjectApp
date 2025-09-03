using CompanyProjectApp.Dtos.ClientManagementDtos;

namespace CompanyProjectApp.Services.ClientManagementServices;

public interface IClientManagementService
{
    public Task<AddPhysicalClientDto> AddNewPhysicalClient(AddPhysicalClientDto physicalClient,
        CancellationToken cancellationToken);

    public Task<int> DeletePhysicalClient(string physicalClientPesel, CancellationToken cancellationToken);

    public Task<AddPhysicalClientDto> ModifyPhysicalClient(string physicalClientPesel,
        ModifyPhysicalClientDto physicalClientDto,
        CancellationToken cancellationToken);

    public Task<AddCompanyClientDto> AddNewCompanyClient(AddCompanyClientDto companyClientDto,
        CancellationToken cancellationToken);

    public Task<AddCompanyClientDto> ModifyCompanyClient(string krsNumber,
        ModifyCompanyClientDto companyClientDto,
        CancellationToken cancellationToken);
}