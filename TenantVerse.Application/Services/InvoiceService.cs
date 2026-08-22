using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IInvoicePdfService _invoicePdfService;

    public InvoiceService(IInvoiceRepository invoiceRepository, IInvoicePdfService invoicePdfService)
    {
        _invoiceRepository = invoiceRepository;
        _invoicePdfService = invoicePdfService;
    }

    public async Task<ApiResponse<CreateInvoiceResponse>> CreateAsync(
        CreateInvoiceRequest request)
    {
        try
        {
            if (request == null)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Invoice request is required."
                };
            }

            if (request.PropertyId <= 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Property is required."
                };
            }

            if (request.UnitId <= 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Flat is required."
                };
            }

            if (request.TenantId <= 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Tenant is required."
                };
            }

            if (request.BillingMonth == default)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Billing month is required."
                };
            }

            if (request.InvoiceDate == default)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Invoice date is required."
                };
            }

            if (request.DueDate == default)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Due date is required."
                };
            }

            if (request.DueDate.Date < request.InvoiceDate.Date)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Due date cannot be earlier than invoice date."
                };
            }

            if (request.MonthlyRent < 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Monthly rent cannot be negative."
                };
            }

            if (request.PreviousReading < 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Previous electricity reading cannot be negative."
                };
            }

            if (request.CurrentReading < request.PreviousReading)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Current electricity reading cannot be less than previous reading."
                };
            }

            if (request.ElectricityRate < 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Electricity rate cannot be negative."
                };
            }

            if (request.MaintenanceCharge < 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Maintenance charge cannot be negative."
                };
            }

            if (request.WaterCharge < 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Water charge cannot be negative."
                };
            }

            if (request.LateFee < 0)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Late fee cannot be negative."
                };
            }

            var result =
                await _invoiceRepository.CreateAsync(request);

            if (result == null)
            {
                return new ApiResponse<CreateInvoiceResponse>
                {
                    IsSuccess = false,
                    Message = "Unable to create invoice."
                };
            }

            return new ApiResponse<CreateInvoiceResponse>
            {
                IsSuccess = true,
                Message = "Invoice created successfully.",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<CreateInvoiceResponse>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<InvoiceModel>>> GetAllAsync()
    {
        try
        {
            var invoices =
                await _invoiceRepository.GetAllAsync();

            return new ApiResponse<IEnumerable<InvoiceModel>>
            {
                IsSuccess = true,
                Message = "Invoices retrieved successfully.",
                Data = invoices
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<InvoiceModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }



public async Task<ApiResponse<int>> UpdateAsync(
    UpdateInvoiceRequest request)
{
    try
    {
        if (request.InvoiceId <= 0)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = "Invalid invoice ID."
            };
        }

        if (request.Charges == null ||
            request.Charges.Count == 0)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = "At least one invoice charge is required."
            };
        }

        var invoiceId =
            await _invoiceRepository.UpdateAsync(request);

        return new ApiResponse<int>
        {
            IsSuccess = true,
            Message = "Invoice updated successfully.",
            Data = invoiceId
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<int>
        {
            IsSuccess = false,
            Message = ex.Message
        };
    }
}



public async Task<ApiResponse<InvoiceModel>> GetByIdAsync(int invoiceId)
{
    try
    {
        if (invoiceId <= 0)
        {
            return new ApiResponse<InvoiceModel>
            {
                IsSuccess = false,
                Message = "Invalid invoice ID."
            };
        }

        var invoice =
            await _invoiceRepository.GetByIdAsync(invoiceId);

        if (invoice == null)
        {
            return new ApiResponse<InvoiceModel>
            {
                IsSuccess = false,
                Message = "Invoice not found."
            };
        }

        return new ApiResponse<InvoiceModel>
        {
            IsSuccess = true,
            Message = "Invoice retrieved successfully.",
            Data = invoice
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<InvoiceModel>
        {
            IsSuccess = false,
            Message = ex.Message
        };
    }
}




public async Task<IEnumerable<InvoiceChargeModel>>GetChargesByInvoiceIdAsync(int invoiceId)
{
    var charges = await _invoiceRepository.GetChargesByInvoiceIdAsync(invoiceId);
    return charges ?? Enumerable.Empty<InvoiceChargeModel>();
}


public async Task<byte[]> GetInvoicePdfAsync(int invoiceId)
{
    var invoice = await _invoiceRepository.GetByIdAsync(invoiceId);
    if (invoice == null)
    {
        throw new KeyNotFoundException(
            $"Invoice with ID {invoiceId} was not found.");
    }
    return await _invoicePdfService.GenerateInvoicePdfAsync(invoice);
}

}