using CompanyProjectApp.Entities;
using Microsoft.EntityFrameworkCore;

namespace CompanyProjectAppTests.Setup;

public class CompanyProjectAppDbContextForTestsFactory
{
    public static CompanyProjectAppContext CreateDbContextForInMemory()
    {
        var options = new DbContextOptionsBuilder<CompanyProjectAppContext>()
            .UseInMemoryDatabase("TestDatabase" + Guid.NewGuid())
            .Options;

        var dbContext = new CompanyProjectAppContext(options);

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}