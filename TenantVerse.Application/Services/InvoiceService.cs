using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IInvoiceRepository _invoiceRepository;

    public InvoiceService(
        IInvoiceRepository invoiceRepository)
    {
        _invoiceRepository = invoiceRepository;
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



}