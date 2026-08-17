using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Infrastructure.Persistence;
using TenantVerse.Infrastructure.Repositories;
using TenantVerse.Application.Interfaces.Authentication;
using TenantVerse.Infrastructure.Repositories.Authentication;
using TenantVerse.Infrastructure.Services.Authentication;
namespace TenantVerse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();
        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUnitRepository, UnitRepository>();
        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();

        return services;
    }
}