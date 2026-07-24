namespace TenantVerse.Shared.Models.Authentication.Responses;

public class LoginResponse
{
    public int UserId { get; set; }

    public string UserCode { get; set; } = string.Empty;

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
    public string Token { get; set; } = string.Empty;

    
}