namespace TenantVerse.Shared.Models.Authentication.Responses;

public class RegisterResponse
{
    public int UserId { get; set; }

    public string UserCode { get; set; } = string.Empty;
}