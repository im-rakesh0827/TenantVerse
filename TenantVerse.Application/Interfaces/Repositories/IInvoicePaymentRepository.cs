using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Interfaces.Repositories;

public interface IInvoicePaymentRepository
{
    Task<CreateInvoicePaymentResponse?> CreateAsync(
        CreateInvoicePaymentRequest request);

    Task<IEnumerable<InvoicePaymentModel>> GetByInvoiceIdAsync(
        int invoiceId);
}