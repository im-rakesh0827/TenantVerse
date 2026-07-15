using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantVerse.Shared.Models.Common
{
    public class BaseEntity
    {
        public bool IsActive { get; set; } = true;

        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

        public string? CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

        public string? UpdatedBy { get; set; } = "Test User";
    }
}

