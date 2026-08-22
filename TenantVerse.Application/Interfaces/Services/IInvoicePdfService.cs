using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Interfaces.Services;

public interface IInvoicePdfService
{
    byte[] GenerateInvoicePdf(InvoiceModel invoice, IEnumerable<InvoiceChargeModel> charges, IEnumerable<InvoicePaymentModel> payments);
    Task<byte[]> GenerateInvoicePdfAsync(InvoiceModel invoice);
}