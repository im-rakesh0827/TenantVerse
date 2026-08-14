using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Unit.Requests;

namespace TenantVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class UnitController : ControllerBase
{
    private readonly IUnitService _unitService;

    public UnitController(IUnitService unitService)
    {
        _unitService = unitService;
    }


    // GET: api/unit
    [HttpGet("allUnits")]
    public async Task<ActionResult<ApiResponse<List<UnitModel>>>> GetAll()
    {
        var response = await _unitService.GetAllAsync();

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


    // GET: api/unit/5
    [HttpGet("getById/{unitId:int}")]
    public async Task<ActionResult<ApiResponse<UnitModel>>> GetById(
        int unitId)
    {
        var response = await _unitService.GetByIdAsync(unitId);

        if (!response.IsSuccess)
            return NotFound(response);

        return Ok(response);
    }


    // GET: api/unit/property/5
    [HttpGet("property/{propertyId:int}")]
    public async Task<ActionResult<ApiResponse<List<UnitModel>>>> GetByPropertyId(
        int propertyId)
    {
        var response =
            await _unitService.GetByPropertyIdAsync(propertyId);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


    // POST: api/unit
    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<int>>> Create(
        [FromBody] CreateUnitRequest request)
    {
        var response =
            await _unitService.CreateAsync(request);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


    // PUT: api/unit
    [HttpPut("update")]
    public async Task<ActionResult<ApiResponse<int>>> Update([FromBody] UpdateUnitRequest request)
    {
        var response =
            await _unitService.UpdateAsync(request);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }


    // DELETE: api/unit/5
    // [HttpDelete("delete/{unitId:int}")]
    // public async Task<ActionResult<ApiResponse<int>>> Delete(int unitId)
    // {
    //     var response =
    //         await _unitService.DeleteAsync(unitId);

    //     if (!response.IsSuccess)
    //         return BadRequest(response);

    //     return Ok(response);
    // }

    [HttpDelete("delete/{unitId:int}")]
    public async Task<ActionResult<ApiResponse<int>>> Delete(
        int unitId)
    {
        var updatedBy = User.Identity?.Name ?? "System";
        var result = await _unitService.DeleteAsync(unitId, updatedBy);

        return Ok(result);
    }
}