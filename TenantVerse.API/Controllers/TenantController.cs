using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models.Tenant;

namespace TenantVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantController : ControllerBase
{
    private readonly ITenantService _tenantService;

    public TenantController(ITenantService tenantService)
    {
        _tenantService = tenantService;
    }

    [HttpGet("getAllTenant")]
    public async Task<IActionResult> GetAll()
    {
        var response = await _tenantService.GetAllAsync();

        return Ok(response);
    }

    [HttpGet("getTenantById/{tenantId:int}")]
    public async Task<IActionResult> GetById(int tenantId)
    {
        var response = await _tenantService.GetByIdAsync(tenantId);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }

    [HttpGet("property/{propertyId:int}")]
    public async Task<IActionResult> GetByPropertyId(int propertyId)
    {
        var response =
            await _tenantService.GetByPropertyIdAsync(propertyId);

        return Ok(response);
    }

    [HttpGet("getByUnitId/{unitId:int}")]
    public async Task<IActionResult> GetByUnitId(int unitId)
    {
        var response =
            await _tenantService.GetByUnitIdAsync(unitId);

        return Ok(response);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateTenantRequest request)
    {
        var response =
            await _tenantService.CreateAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateTenantRequest request)
    {
        var response =
            await _tenantService.UpdateAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }

    [HttpDelete("{tenantId:int}")]
    public async Task<IActionResult> Delete(
        int tenantId,
        [FromQuery] string updatedBy)
    {
        var response =
            await _tenantService.DeleteAsync(
                tenantId,
                updatedBy);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}