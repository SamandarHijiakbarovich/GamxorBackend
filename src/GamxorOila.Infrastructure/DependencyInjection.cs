using GamxorOila.Application.Common.Interfaces;
using GamxorOila.Infrastructure.Persistence;
using GamxorOila.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GamxorOila.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var connectionString = ConnectionStringResolver.Resolve(config);

        services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connectionString));

        services.Configure<OtpOptions>(config.GetSection(OtpOptions.Section));
        services.Configure<FileStorageOptions>(config.GetSection(FileStorageOptions.Section));

        services.AddScoped<IDeviceRepository, DeviceRepository>();
        services.AddScoped<IFileStorage, LocalFileStorage>();
        services.AddScoped<IOtpGenerator, OtpGenerator>();
        services.AddSingleton<IClock, SystemClock>();

        return services;
    }
}
