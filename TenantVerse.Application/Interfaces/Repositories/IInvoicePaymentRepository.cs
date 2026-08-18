using TenantVerse.Shared.Models.Invoice;
using TenantVerse.Shared.Models;
namespace TenantVerse.Application.Interfaces.Repositories;

public interface IInvoicePaymentRepository
{
    Task<CreateInvoicePaymentResponse?> CreateAsync(
        CreateInvoicePaymentRequest request);

    Task<IEnumerable<InvoicePaymentModel>> GetByInvoiceIdAsync(
        int invoiceId);

     Task<ApiResponse<ReverseInvoicePaymentResponse>> ReverseAsync(
    ReverseInvoicePaymentRequest request);
}