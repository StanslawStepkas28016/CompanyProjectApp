using CompanyProjectApp.Dtos.AppUserDtos;
using CompanyProjectApp.Services.AuthenticationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CompanyProjectApp.Controllers;

/// <summary>
///     Controller responsible for presenting the data and providing implementation for following use cases,
///     RegisterAppUser, LoginAppUser and RefreshAppUserToken.
/// </summary>
[Route("api/authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private IAuthenticationService _service;

    public AuthenticationController(IAuthenticationService service)
    {
        _service = service;
    }

    /// <summary>
    ///     Endpoint used for registering a user of the application.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterAppUser(AppUserRegisterRequest user, CancellationToken cancellationToken)
    {
        await _service.RegisterAppUser(user, cancellationToken);
        return Ok("Successfully registered");
    }

    /// <summary>
    ///     Endpoint used for logging a user of the application.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> LoginAppUser(AppUserLoginRequest user, CancellationToken cancellationToken)
    {
        var res = await _service.LoginAppUser(user, cancellationToken);
        return Ok(res);
    }

    /// <summary>
    ///     Endpoint used for refreshing a user provided token.
    /// </summary>
    /// <param name="tokenRequest"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [Authorize(AuthenticationSchemes = "IgnoreTokenExpirationScheme")]
    [HttpPost("token/refresh")]
    public async Task<IActionResult> RefreshAppUserToken(AppUserTokenRefreshRequest tokenRequest,
        CancellationToken cancellationToken)
    {
        var res = await _service.RefreshAppUserToken(tokenRequest, cancellationToken);
        return Ok(res);
    }
}