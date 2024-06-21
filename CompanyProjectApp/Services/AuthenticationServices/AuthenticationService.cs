using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CompanyProjectApp.Dtos.AppUserDtos;
using CompanyProjectApp.Entities;
using CompanyProjectApp.Entities.AppUserEntities;
using CompanyProjectApp.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CompanyProjectApp.Services.AuthenticationServices;

public class AuthenticationService : IAuthenticationService
{
    private readonly CompanyProjectAppContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationService(CompanyProjectAppContext context)
    {
        _context = context;
        _configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();
    }

    public async Task<int> RegisterAppUser(AppUserRegisterRequest request, CancellationToken cancellationToken)
    {
        if (await DoesUserExistBasedOnLogin(request.Login, cancellationToken))
        {
            throw new ArgumentException("User with the provided login already exist! Choose another username!");
        }

        if (IsProvidedRoleWithinTheAppConstraints(request.Role) == false)
        {
            throw new ArgumentException("Role can be only: \'admin\' or \'regular\'!");
        }

        var hashedPasswordAndSalt = SecurityHelper.GetHashedPasswordAndSalt(request.Password);

        var user = new AppUser
        {
            Login = request.Login,
            Password = hashedPasswordAndSalt.Item1,
            Role = request.Role,
            Salt = hashedPasswordAndSalt.Item2,
            RefreshToken = SecurityHelper.GenerateRefreshToken(),
            RefreshTokenExp = DateTime.Now.AddDays(1),
        };

        await _context
            .AppUsers
            .AddAsync(user, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return 1;
    }

    public async Task<AccesAndRefreshTokenResponse> LoginAppUser(AppUserLoginRequest request,
        CancellationToken cancellationToken)
    {
        if (await DoesUserExistBasedOnLogin(request.Login, cancellationToken) == false)
        {
            throw new ArgumentException("User with the provided Login does not exist!");
        }

        if (await IsTheProvidedPasswordRight(request, cancellationToken) == false)
        {
            throw new ArgumentException("Provided password is incorrect!");
        }

        var user = await _context
            .AppUsers
            .Where(aa => aa.Login == request.Login)
            .FirstOrDefaultAsync(cancellationToken);

        var userClaim = new[]
        {
            new Claim(ClaimTypes.Name, request.Login),
            new Claim(ClaimTypes.Role, user!.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: userClaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: credentials
        );

        user.RefreshToken = SecurityHelper.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);

        await _context.SaveChangesAsync(cancellationToken);

        return new AccesAndRefreshTokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken
        };
    }

    public async Task<AccesAndRefreshTokenResponse> RefreshAppUserToken(AppUserTokenRefreshRequest tokenRequest,
        CancellationToken cancellationToken)
    {
        if (await DoesUserExistBasedOnRefreshToken(tokenRequest.RefreshToken, cancellationToken) == false)
        {
            throw new ArgumentException("User with the provided refresh token does not exist!");
        }

        if (await HasRefreshTokenExpired(tokenRequest.RefreshToken, cancellationToken))
        {
            throw new ArgumentException("The provided refresh token has expired!");
        }

        var user = await _context
            .AppUsers
            .Where(aa => aa.RefreshToken == tokenRequest.RefreshToken)
            .FirstOrDefaultAsync(cancellationToken);

        var userClaim = new[]
        {
            new Claim(ClaimTypes.Name, user!.Login),
            new Claim(ClaimTypes.Role, user.Role),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: userClaim,
            expires: DateTime.Now.AddMinutes(10),
            signingCredentials: credentials
        );

        user.RefreshToken = SecurityHelper.GenerateRefreshToken();
        user.RefreshTokenExp = DateTime.Now.AddDays(1);

        await _context.SaveChangesAsync(cancellationToken);

        return new AccesAndRefreshTokenResponse
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = user.RefreshToken
        };
    }

    private async Task<bool> IsTheProvidedPasswordRight(AppUserLoginRequest request,
        CancellationToken cancellationToken)
    {
        var user = await _context
            .AppUsers
            .Where(aa => aa.Login == request.Login)
            .FirstOrDefaultAsync(cancellationToken);

        var passwordFromDatabase = user!.Password;
        var requestHashedPasswordWithSalt = SecurityHelper.GetHashedPasswordWithSalt(request.Password, user.Salt);

        return passwordFromDatabase == requestHashedPasswordWithSalt;
    }

    private async Task<bool> DoesUserExistBasedOnLogin(string login, CancellationToken cancellationToken)
    {
        var res = await _context
            .AppUsers
            .Where(aa => aa.Login == login)
            .FirstOrDefaultAsync(cancellationToken);

        return res != null;
    }

    private static readonly List<string> AvailableRoles = ["admin", "regular"];

    private bool IsProvidedRoleWithinTheAppConstraints(string role)
    {
        return AvailableRoles.Contains(role);
    }

    private async Task<bool> DoesUserExistBasedOnRefreshToken(string refreshToken, CancellationToken cancellationToken)
    {
        var res = await _context
            .AppUsers
            .Where(aa => aa.RefreshToken == refreshToken)
            .FirstOrDefaultAsync(cancellationToken);

        return res != null;
    }

    private async Task<bool> HasRefreshTokenExpired(string refreshToken, CancellationToken cancellationToken)
    {
        var res = await _context
            .AppUsers
            .Where(aa => aa.RefreshToken == refreshToken)
            .FirstOrDefaultAsync(cancellationToken);

        return res!.RefreshTokenExp < DateTime.Now;
    }
}