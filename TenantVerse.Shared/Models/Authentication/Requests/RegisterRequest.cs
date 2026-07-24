namespace TenantVerse.Shared.Models.Authentication.Requests;

public class RegisterRequest
{
    public string FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;
}

