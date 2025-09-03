using CompanyProjectApp.Dtos.AppUserDtos;

namespace CompanyProjectApp.Services.AuthenticationServices;

public interface IAuthenticationService
{
    public Task<int> RegisterAppUser(AppUserRegisterRequest request, CancellationToken cancellationToken);

    public Task<AccesAndRefreshTokenResponse> LoginAppUser(AppUserLoginRequest request,
        CancellationToken cancellationToken);

    public Task<AccesAndRefreshTokenResponse> RefreshAppUserToken(AppUserTokenRefreshRequest tokenRequest,
        CancellationToken cancellationToken);
}