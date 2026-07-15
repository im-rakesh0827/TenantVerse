using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.Application.DTOs;
namespace TenantVerse.Application.Interfaces
{
    public interface IHealthService
    {
            Task<HealthResponseDto> GetHealthAsync();
            Task<string> GetStatusAsync();
    }
}