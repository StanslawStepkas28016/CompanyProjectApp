using CompanyProjectApp.Dtos.AppUserDtos;
using CompanyProjectApp.Entities;
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
}