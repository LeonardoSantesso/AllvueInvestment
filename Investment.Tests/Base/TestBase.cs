using AutoMapper;
using Investment.Core.Mappings;
using Investment.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace Investment.Tests.Base;

public abstract class TestBase : IDisposable
{
    private readonly DbContextOptions<InvestmentDbContext> _options;

    protected TestBase()
    {
        _options = new DbContextOptionsBuilder<InvestmentDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
    }

    protected IMapper CreateMapper()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ApplicationProfile());
        });

        return config.CreateMapper();
    }

    protected InvestmentDbContext CreateDbContext()
    {
        var context = new InvestmentDbContext(_options);

        context.Database.EnsureCreated();
        return context;
    }

    public void Dispose()
    {
        using var context = new InvestmentDbContext(_options);
        context.Database.EnsureDeleted();
    }
}
