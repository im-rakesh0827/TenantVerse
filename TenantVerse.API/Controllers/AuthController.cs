using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.Interfaces.Authentication;
using TenantVerse.Shared.Models.Authentication.Requests;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
namespace TenantVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Register New User
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var response = await _authService.RegisterAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }


    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var response = await _authService.LoginAsync(request);

        if (response.IsSuccess)
        {
            return Ok(response);
        }

        return BadRequest(response);
    }

    [Authorize]
[HttpGet("me")]
public IActionResult Me()
{
    return Ok(new
    {
        UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
        Email = User.FindFirstValue(ClaimTypes.Email),
        Name = User.FindFirstValue(ClaimTypes.Name),
        Role = User.FindFirstValue(ClaimTypes.Role),
        UserCode = User.FindFirst("UserCode")?.Value
    });
}
    
}