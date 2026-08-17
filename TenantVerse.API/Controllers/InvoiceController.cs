using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// [Authorize]
public class InvoiceController : ControllerBase
{
    private readonly IInvoiceService _invoiceService;

    public InvoiceController(
        IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<ApiResponse<CreateInvoiceResponse>>> Create(
        [FromBody] CreateInvoiceRequest request)
    {
        var userName =
            User.Identity?.Name ?? "System";

        request.CreatedBy = userName;

        var result =
            await _invoiceService.CreateAsync(request);

        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }

    [HttpGet("getAll")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceModel>>>> GetAll()
    {
        var result = await _invoiceService.GetAllAsync();
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }

    [HttpPut("{invoiceId:int}")]
public async Task<IActionResult> Update(
    int invoiceId,
    [FromBody] UpdateInvoiceRequest request)
{
    try
    {
        if (invoiceId <= 0)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "Invalid invoice ID."
            });
        }

        if (request == null)
        {
            return BadRequest(new
            {
                IsSuccess = false,
                Message = "Invoice request is required."
            });
        }

        // Ensure route ID and request ID are consistent
        request.InvoiceId = invoiceId;

        var result =
            await _invoiceService.UpdateAsync(request);

        return Ok(new
        {
            IsSuccess = true,
            Message = "Invoice updated successfully.",
            Data = new
            {
                InvoiceId = result
            }
        });
    }
    catch (ArgumentException ex)
    {
        return BadRequest(new
        {
            IsSuccess = false,
            Message = ex.Message
        });
    }
    catch (Exception)
    {
        return StatusCode(
            StatusCodes.Status500InternalServerError,
            new
            {
                IsSuccess = false,
                Message = "An error occurred while updating the invoice."
            });
    }
}
}