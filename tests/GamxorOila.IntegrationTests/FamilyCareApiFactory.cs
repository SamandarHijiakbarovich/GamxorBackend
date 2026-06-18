using GamxorOila.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GamxorOila.IntegrationTests;

/// <summary>
/// Npgsql o'rniga SQLite in-memory bazani ulaydigan test fabrikasi.
/// SQLite relyatsion provayder bo'lgani uchun Postgres xatti-harakatiga (owned type, FK)
/// InMemory provayderga qaraganda yaqinroq. Test uchun haqiqiy Postgres talab qilinmaydi.
/// </summary>
public class FamilyCareApiFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public FamilyCareApiFactory() => _connection.Open();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            var descriptors = services
                .Where(d =>
                    d.ServiceType == typeof(AppDbContext) ||
                    d.ServiceType == typeof(DbContextOptions) ||
                    d.ServiceType == typeof(DbContextOptions<AppDbContext>) ||
                    (d.ServiceType.FullName?.Contains("IDbContextOptionsConfiguration") ?? false) ||
                    (d.ServiceType.FullName?.Contains("Npgsql") ?? false) ||
                    (d.ImplementationType?.FullName?.Contains("Npgsql") ?? false))
                .ToList();
            foreach (var d in descriptors) services.Remove(d);

            services.AddDbContext<AppDbContext>(o => o.UseSqlite(_connection));
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);
        using var scope = host.Services.CreateScope();
        scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreated();
        return host;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing) _connection.Dispose();
    }
}
