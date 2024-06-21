using CompanyProjectApp.Dtos.AgreementDtos;
using CompanyProjectApp.Dtos.ProductClientDtos;

namespace CompanyProjectApp.Services.AgreementServices;

public interface IAgreementService
{
    public Task<CreateAgreementResponseDto> CreateAgreement(CreateAgreementRequestDto request,
        CancellationToken cancellationToken);

    public Task<PayForAgreementResponseDto> PayForAgreement(PayForAgreementRequestDto request,
        CancellationToken cancellationToken);
}