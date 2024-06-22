using CompanyProjectApp.Dtos.AppUserDtos;
using CompanyProjectApp.Entities;
using CompanyProjectApp.Entities.AppUserEntities;
using CompanyProjectAppTests.Setup;
using Microsoft.EntityFrameworkCore;
using Assert = NUnit.Framework.Assert;

namespace CompanyProjectAppTests.AuthenticationService;

public class AuthenticationServiceTests
{
    private readonly CompanyProjectAppContext _context;
    private readonly CompanyProjectApp.Services.AuthenticationServices.AuthenticationService _service;

    public AuthenticationServiceTests()
    {
        _context = CompanyProjectAppDbContextForTestsFactory.CreateDbContextForInMemory();
        _service = new CompanyProjectApp.Services.AuthenticationServices.AuthenticationService(_context);
    }

    [Fact]
    public async Task RegisterUserShouldAddUserToDatabase()
    {
        await _service.RegisterAppUser(new AppUserRegisterRequest
        {
            Login = "John",
            Password = "Doe",
            Role = "admin"
        }, new CancellationToken());

        await _context.SaveChangesAsync();

        var count = await _context.AppUsers.CountAsync();
        Assert.That(count, Is.EqualTo(1));
    }

    [Fact]
    public Task RegisterUserWithAnIncorrectRoleShouldThrowException()
    {
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.RegisterAppUser(new AppUserRegisterRequest
            {
                Login = "John",
                Password = "Doe",
                Role = "imaginary_role"
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("Role can be only: 'admin' or 'regular'!"));
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RegisterUserWithAnAlreadyExistingLoginShouldThrowException()
    {
        await _service.RegisterAppUser(new AppUserRegisterRequest
        {
            Login = "John",
            Password = "Doe",
            Role = "admin"
        }, new CancellationToken());

        await _context.SaveChangesAsync();

        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.RegisterAppUser(new AppUserRegisterRequest
            {
                Login = "John",
                Password = "NotDoe",
                Role = "user"
            }, new CancellationToken());
        });

        Assert.That(exception.Message,
            Is.EqualTo("User with the provided login already exist! Choose another username!"));
    }

    [Fact]
    public async Task LoginUserShouldReturnAccessToken()
    {
        await _service.RegisterAppUser(
            new AppUserRegisterRequest { Login = "admin", Password = "admin", Role = "admin" },
            new CancellationToken());

        var response = await _service.LoginAppUser(new AppUserLoginRequest { Login = "admin", Password = "admin" },
            new CancellationToken());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.AccessToken, Is.Not.Null);
            Assert.That(string.IsNullOrEmpty(response.AccessToken), Is.False);
        });
    }

    [Fact]
    public Task LoginUserShouldThrowExceptionWhenNoLoginIsAssociatedWithAnyUser()
    {
        var exception = Assert.ThrowsAsync<ArgumentException>(async () =>
        {
            await _service.LoginAppUser(new AppUserLoginRequest()
            {
                Login = "John",
                Password = "Doe",
            }, new CancellationToken());
        });

        Assert.That(exception.Message, Is.EqualTo("User with the provided Login does not exist!"));
        return Task.CompletedTask;
    }

    [Fact]
    public async Task RefreshTokenShouldReturnAccessRefreshToken()
    {
        var user = new AppUser
        {
            Login = "admin",
            Password = "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=",
            Role = "admin",
            Salt = "fUYAuxqW54H8a69VlzikJg==",
            RefreshToken = "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=",
            RefreshTokenExp = new DateTime(2024, 12, 20)
        };

        await _context.AppUsers.AddAsync(
            user
        );

        await _context.SaveChangesAsync();

        var response = await _service.RefreshAppUserToken(new AppUserTokenRefreshRequest
        {
            RefreshToken = user.RefreshToken
        }, new CancellationToken());

        Assert.Multiple(() =>
        {
            Assert.That(response, Is.Not.Null);
            Assert.That(response.RefreshToken, Is.Not.Null);
            Assert.That(string.IsNullOrEmpty(response.RefreshToken), Is.False);
        });
    }
}