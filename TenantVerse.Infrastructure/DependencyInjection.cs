using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Infrastructure.Persistence;
using TenantVerse.Infrastructure.Repositories;
namespace TenantVerse.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
        services.AddScoped<IPropertyRepository, PropertyRepository>();

        return services;
    }
}