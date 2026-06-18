using GamxorOila.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GamxorOila.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFamilyCareService, FamilyCareService>();
        return services;
    }
}
