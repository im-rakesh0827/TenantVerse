using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using TenantVerse.Application.Interfaces;
using TenantVerse.Application.Services;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Application.Services;


namespace TenantVerse.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IHealthService, HealthService>();
            services.AddScoped<IPropertyService, PropertyService>();

            return services;
        }
    }
}