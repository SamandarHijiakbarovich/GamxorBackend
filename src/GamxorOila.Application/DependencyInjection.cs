using FluentValidation;
using GamxorOila.Application.Services;
using GamxorOila.Application.Validation;
using Microsoft.Extensions.DependencyInjection;

namespace GamxorOila.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IFamilyCareService, FamilyCareService>();
        services.AddValidatorsFromAssemblyContaining<RequestCodeRequestValidator>();
        return services;
    }
}
