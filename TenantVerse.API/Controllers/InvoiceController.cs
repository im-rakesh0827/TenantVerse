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

    [HttpGet("all")]
    public async Task<ActionResult<ApiResponse<IEnumerable<InvoiceListModel>>>> GetAll()
    {
        var result = await _invoiceService.GetAllAsync();
        if (!result.IsSuccess)
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}