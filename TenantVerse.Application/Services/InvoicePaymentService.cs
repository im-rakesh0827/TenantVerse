using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Application.Services;

public class InvoicePaymentService : IInvoicePaymentService
{
    private readonly IInvoicePaymentRepository _repository;

    public InvoicePaymentService(
        IInvoicePaymentRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<CreateInvoicePaymentResponse>> CreateAsync(
        CreateInvoicePaymentRequest request)
    {
        try
        {
            var result =
                await _repository.CreateAsync(request);

            if (result is null)
            {
                return new ApiResponse<CreateInvoicePaymentResponse>
                {
                    IsSuccess = false,
                    Message = "Unable to process payment."
                };
            }

            return new ApiResponse<CreateInvoicePaymentResponse>
            {
                IsSuccess = true,
                Message = "Payment recorded successfully.",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<CreateInvoicePaymentResponse>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }

    public async Task<ApiResponse<IEnumerable<InvoicePaymentModel>>> GetByInvoiceIdAsync(
        int invoiceId)
    {
        try
        {
            var result =
                await _repository.GetByInvoiceIdAsync(invoiceId);

            return new ApiResponse<IEnumerable<InvoicePaymentModel>>
            {
                IsSuccess = true,
                Message = "Payment history retrieved successfully.",
                Data = result
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<IEnumerable<InvoicePaymentModel>>
            {
                IsSuccess = false,
                Message = ex.Message
            };
        }
    }


public async Task<ApiResponse<ReverseInvoicePaymentResponse>> ReverseAsync(
    ReverseInvoicePaymentRequest request)
{
    return await _repository.ReverseAsync(request);
}



public async Task<ApiResponse<IEnumerable<InvoicePaymentModel>>> GetAllPaymentAsync()
{
    try
    {
        var payments = await _repository.GetAllPaymentAsync();

        return new ApiResponse<IEnumerable<InvoicePaymentModel>>
        {
            IsSuccess = true,
            Message = "Invoice payments retrieved successfully.",
            Data = payments
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<IEnumerable<InvoicePaymentModel>>
        {
            IsSuccess = false,
            Message = ex.Message,
            Data = Enumerable.Empty<InvoicePaymentModel>()
        };
    }
}
}