using CompanyProjectApp.Dtos.ClientManagementDtos;

namespace CompanyProjectApp.Services.ClientRepositories;

public interface IClientManagementService
{
    public Task<RegisterPhysicalClientDto> AddNewPhysicalClient(RegisterPhysicalClientDto physicalClient,
        CancellationToken cancellationToken);

    public Task<int> DeletePhysicalClient(string physicalClientPesel, CancellationToken cancellationToken);

    public Task<RegisterPhysicalClientDto> ModifyPhysicalClient(string physicalClientPesel,
        ModifyPhysicalClientDto physicalClientDto,
        CancellationToken cancellationToken);

    public Task<RegisterCompanyClientDto> AddNewCompanyClient(RegisterCompanyClientDto companyClientDto,
        CancellationToken cancellationToken);

    public Task<RegisterCompanyClientDto> ModifyCompanyClient(string krsNumber,
        ModifyCompanyClientDto companyClientDto,
        CancellationToken cancellationToken);
}