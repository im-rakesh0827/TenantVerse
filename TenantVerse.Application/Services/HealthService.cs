using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.Application.DTOs;
using TenantVerse.Application.Interfaces;
namespace TenantVerse.Application.Services
{
    public class HealthService : IHealthService
    {
        public Task<HealthResponseDto> GetHealthAsync()
    {
        var response = new HealthResponseDto
        {
            ApplicationName = "TenantVerse",
            Version = "1.0.0",
            Environment = "Development",
            ServerTime = DateTime.UtcNow
        };

        return Task.FromResult(response);
    }


    public Task<string> GetStatusAsync()
    {
        return Task.FromResult("TenantVerse API is running successfully.");
    }
    }
}