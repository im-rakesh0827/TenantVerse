using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantVerse.Shared.Models.Authentication
{
    public class UserLoginData
{
    public int UserId { get; set; }

    public string UserCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PasswordHash { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
    // public string Token { get; set; } = string.Empty;


    public bool IsActive { get; set; }

    public bool IsDeleted { get; set; }
}
}