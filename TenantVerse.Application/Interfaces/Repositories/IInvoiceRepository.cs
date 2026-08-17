using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Interfaces.Repositories;

public interface IInvoiceRepository
{
//     Task<int> CreateAsync(CreateInvoiceRequest request);
     Task<CreateInvoiceResponse?> CreateAsync(
        CreateInvoiceRequest request);
     Task<IEnumerable<InvoiceModel>> GetAllAsync();

     Task<int> UpdateAsync(UpdateInvoiceRequest request);
   
}