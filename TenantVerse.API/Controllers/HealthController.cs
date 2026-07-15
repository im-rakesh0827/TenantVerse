using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.Interfaces;
using TenantVerse.Shared.Models;

namespace TenantVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    private readonly IHealthService _healthService;

    public HealthController(IHealthService healthService)
    {
        _healthService = healthService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<object>>> Get()
    {
        var health = await _healthService.GetHealthAsync();

        var response = new ApiResponse<object>
        {
            Success = true,
            Message = "TenantVerse API is running successfully.",
            Data = health
        };

        return Ok(response);
    }

    // [HttpGet]
    // public IActionResult Get()
    // {
    //     throw new Exception("This is a middleware test.");
    // }
}