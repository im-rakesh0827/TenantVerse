using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Interfaces.Services;

public interface IInvoiceService
{
    Task<ApiResponse<CreateInvoiceResponse>> CreateAsync(
        CreateInvoiceRequest request);
        Task<ApiResponse<IEnumerable<InvoiceModel>>> GetAllAsync();

        // Task<int> UpdateAsync(
        // UpdateInvoiceRequest request);

        Task<ApiResponse<int>> UpdateAsync(
    UpdateInvoiceRequest request);

    Task<ApiResponse<InvoiceModel>> GetByIdAsync(
    int invoiceId);
}