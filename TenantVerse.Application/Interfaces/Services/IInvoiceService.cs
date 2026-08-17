using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Interfaces.Services;

public interface IInvoiceService
{
    Task<ApiResponse<CreateInvoiceResponse>> CreateAsync(
        CreateInvoiceRequest request);
        Task<ApiResponse<IEnumerable<InvoiceListModel>>> GetAllAsync();
}