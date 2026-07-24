using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.DTOs.Property;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace TenantVerse.API.Controllers;
// [Authorize]
[ApiController]
[Route("api/[controller]")]
public class PropertyController : ControllerBase
{
    private readonly IPropertyService _propertyService;

    public PropertyController(IPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreatePropertyRequest request)
    {
        var propertyId = await _propertyService.CreateAsync(request);

        var response = new ApiResponse<int>
        {
            IsSuccess = true,
            Message = "Property created successfully.",
            Data = propertyId
        };

        return Ok(response);
    }

    [HttpGet("allProperties")]
    public async Task<IActionResult> GetAll()
    {
        var properties = await _propertyService.GetAllAsync();

        var response = new ApiResponse<IEnumerable<PropertyListResponse>>
        {
            IsSuccess = true,
            Message = "Properties retrieved successfully.",
            Data = properties
        };

        return Ok(response);
    }


    [HttpGet("getById/{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _propertyService.GetByIdAsync(id);

        if (property == null)
        {
            return NotFound(
            ApiResponseHelper.Fail<object>(
                "Property not found."));
        }

        return Ok(
            ApiResponseHelper.Success(
                property,
                "Property retrieved successfully."));
    }


    [HttpPut("update")]
    public async Task<IActionResult> Update(UpdatePropertyRequest request)
    {
        var updated = await _propertyService.UpdateAsync(request);


        if (!updated)
        {
            return NotFound(
                ApiResponseHelper.Fail<object>(
                    "Property not found."));
        }

        return Ok(
            ApiResponseHelper.Success(
                true,
                "Property updated successfully."));
    }


    [HttpDelete("delete/{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _propertyService.DeleteAsync(id, "System");
        if (!deleted)
        {
            return NotFound(
                ApiResponseHelper.Fail<object>(
                    "Property not found."));
        }
        return Ok(
            ApiResponseHelper.Success(
                true,
                "Property deleted successfully."));
    }
}