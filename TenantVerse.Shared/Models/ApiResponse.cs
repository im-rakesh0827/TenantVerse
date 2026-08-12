using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantVerse.Shared.Models
{
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }

        public string Message { get; set; } = string.Empty;

        public T? Data { get; set; }

        public List<string> Errors { get; set; } = new();
    }
}