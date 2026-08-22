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
    // private readonly IInvoiceService _invoiceService;

    // public InvoiceController(
    //     IInvoiceService invoiceService)
    // {
    //     _invoiceService = invoiceService;
    // }


    private readonly IInvoiceService _invoiceService;
    private readonly IInvoicePaymentService _invoicePaymentService;
    private readonly IInvoicePdfService _invoicePdfService;

    public InvoiceController(
        IInvoiceService invoiceService,
        IInvoicePdfService invoicePdfService,
        IInvoicePaymentService invoicePaymentService)
    {
        _invoiceService = invoiceService;
        _invoicePdfService = invoicePdfService;
        _invoicePaymentService = invoicePaymentService;
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

    [HttpGet("getById/{invoiceId:int}")]
    public async Task<IActionResult> GetById(
        int invoiceId)
    {
        var response =
            await _invoiceService.GetByIdAsync(invoiceId);

        if (!response.IsSuccess)
        {
            return NotFound(response);
        }

        return Ok(response);
    }
    

    [HttpPut("update")]
    public async Task<IActionResult> Update(
        [FromBody] UpdateInvoiceRequest request)
    {
        var response =
            await _invoiceService.UpdateAsync(request);

        if (!response.IsSuccess)
        {
            return BadRequest(response);
        }

        return Ok(response);
    }



    [HttpGet("charges/{invoiceId:int}")]
    public async Task<ActionResult<IEnumerable<InvoiceChargeModel>>>
    GetChargesByInvoiceId(int invoiceId)
    {
        var charges =
            await _invoiceService.GetChargesByInvoiceIdAsync(invoiceId);

        return Ok(charges);
    }





    [HttpGet("{invoiceId:int}/pdf")]
    public async Task<IActionResult> GenerateInvoicePdf(int invoiceId)
    {
        try
        {
            // ==========================================
            // GET INVOICE
            // ==========================================

            var invoiceResponse = await _invoiceService.GetByIdAsync(invoiceId);

            if (invoiceResponse is null ||
                !invoiceResponse.IsSuccess ||
                invoiceResponse.Data is null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = invoiceResponse?.Message
                        ?? "Invoice not found."
                });
            }

            var invoice = invoiceResponse.Data;


            // ==========================================
            // GET CHARGES
            // ==========================================

            var charges = await _invoiceService.GetChargesByInvoiceIdAsync(invoiceId);

            charges ??= Enumerable.Empty<InvoiceChargeModel>();


            // ==========================================
            // GET PAYMENTS
            // ==========================================

            var paymentResponse = await _invoicePaymentService.GetByInvoiceIdAsync(invoiceId);

            var payments =
                paymentResponse?.Data
                ?? Enumerable.Empty<InvoicePaymentModel>();


            // ==========================================
            // GENERATE PDF
            // ==========================================

            var pdf =
                _invoicePdfService.GenerateInvoicePdf(
                    invoice,
                    charges,
                    payments);


            // ==========================================
            // RETURN PDF
            // ==========================================

            return File(
                pdf,
                "application/pdf",
                $"Invoice - {invoice.InvoiceNumber}.pdf");
        }
        catch (Exception ex)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new
                {
                    IsSuccess = false,
                    Message = ex.Message
                });
        }
    }






    [HttpGet("generateInvoicePdf/{invoiceId:int}/pdf")]
    public async Task<IActionResult> GetInvoicePdf(int invoiceId)
    {
        try
        {
            var pdf =
                await _invoiceService.GetInvoicePdfAsync(invoiceId);

            return File(
                pdf,
                "application/pdf",
                $"Invoice-{invoiceId}.pdf");
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

}