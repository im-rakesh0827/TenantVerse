using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantVerse.Application.DTOs
{
    public class HealthResponseDto
    {
        public string ApplicationName { get; set; } = string.Empty;

        public string Version { get; set; } = string.Empty;

        public DateTime ServerTime { get; set; }

        public string Environment { get; set; } = string.Empty;
    }
}