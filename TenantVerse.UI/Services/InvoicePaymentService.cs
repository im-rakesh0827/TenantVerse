using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Net.Http.Json;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;


namespace TenantVerse.UI.Services
{
    public class InvoicePaymentService
    {

        private readonly IHttpClientFactory _httpClientFactory;

    public InvoicePaymentService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Client =>
        _httpClientFactory.CreateClient("TenantVerseAPI");

    private string baseUrl =
        "http://localhost:5168/api/InvoicePayment";




public async Task<ApiResponse<CreateInvoicePaymentResponse>> CreateAsync(
    CreateInvoicePaymentRequest request)
{
    var response = await Client.PostAsJsonAsync(
        $"{baseUrl}/create",
        request);

    return await response.Content
               .ReadFromJsonAsync<
                   ApiResponse<CreateInvoicePaymentResponse>>()
           ?? new ApiResponse<CreateInvoicePaymentResponse>
           {
               IsSuccess = false,
               Message = "Unable to process payment."
           };
}


public async Task<ApiResponse<List<InvoicePaymentModel>>> GetByInvoiceIdAsync(
    int invoiceId)
{
    var response = await Client.GetAsync(
        $"{baseUrl}/invoice/{invoiceId}");

    return await response.Content
               .ReadFromJsonAsync<
                   ApiResponse<List<InvoicePaymentModel>>>()
           ?? new ApiResponse<List<InvoicePaymentModel>>
           {
               IsSuccess = false,
               Message = "Unable to load payment history."
           };
}
        




public async Task<ApiResponse<ReverseInvoicePaymentResponse>> ReverseAsync(
    ReverseInvoicePaymentRequest request)
{
    var response = await Client.PostAsJsonAsync(
        $"{baseUrl}/reverse",
        request);

    return await response.Content
               .ReadFromJsonAsync<
                   ApiResponse<ReverseInvoicePaymentResponse>>()
           ?? new ApiResponse<ReverseInvoicePaymentResponse>
           {
               IsSuccess = false,
               Message = "Unable to reverse payment."
           };
}

public async Task<ApiResponse<IEnumerable<InvoicePaymentModel>>> GetAllPaymentAsync()
    {
        try
        {
            var response =
                await Client.GetAsync("{baseUrl}/geAllPayment");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<IEnumerable<InvoicePaymentModel>>
                {
                    IsSuccess = false,
                    Message = $"Failed to retrieve invoice payments. Status code: {response.StatusCode}",
                    Data = Enumerable.Empty<InvoicePaymentModel>()
                };
            }

            var result =
                await response.Content
                    .ReadFromJsonAsync<ApiResponse<IEnumerable<InvoicePaymentModel>>>();

            return result ??
                new ApiResponse<IEnumerable<InvoicePaymentModel>>
                {
                    IsSuccess = false,
                    Message = "No response received from server.",
                    Data = Enumerable.Empty<InvoicePaymentModel>()
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
    
}