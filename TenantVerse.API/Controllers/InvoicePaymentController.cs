using Microsoft.AspNetCore.Mvc;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicePaymentController : ControllerBase
{
    private readonly IInvoicePaymentService _invoicePaymentService;

    public InvoicePaymentController(
        IInvoicePaymentService invoicePaymentService)
    {
        _invoicePaymentService = invoicePaymentService;
    }

    // ============================================
    // CREATE PAYMENT
    // ============================================

    [HttpPost("create")]
    public async Task<IActionResult> Create(
        [FromBody] CreateInvoicePaymentRequest request)
    {
        var response =
            await _invoicePaymentService.CreateAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }


    // ============================================
    // GET PAYMENT HISTORY
    // ============================================

    [HttpGet("invoice/{invoiceId:int}")]
    public async Task<IActionResult> GetByInvoiceId(
        int invoiceId)
    {
        var response =
            await _invoicePaymentService
                .GetByInvoiceIdAsync(invoiceId);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }
}