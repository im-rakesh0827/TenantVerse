using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Interfaces.Services;

public interface IInvoicePaymentService
{
    Task<ApiResponse<CreateInvoicePaymentResponse>> CreateAsync(
        CreateInvoicePaymentRequest request);

    Task<ApiResponse<IEnumerable<InvoicePaymentModel>>> GetByInvoiceIdAsync(
        int invoiceId);
Task<ApiResponse<ReverseInvoicePaymentResponse>> ReverseAsync(
    ReverseInvoicePaymentRequest request);
}