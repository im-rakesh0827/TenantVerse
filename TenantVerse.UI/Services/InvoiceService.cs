using System.Net.Http.Json;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.UI.Services;

public class InvoiceService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public InvoiceService(
        IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Client =>
        _httpClientFactory.CreateClient("TenantVerseAPI");

    private string baseUrl =
        "http://localhost:5168/api/Invoice";


    // =========================================================
    // GET ALL INVOICES
    // =========================================================

    public async Task<ApiResponse<List<InvoiceModel>>> GetAllAsync()
    {
        try
        {
            var response = await Client.GetAsync(
                $"{baseUrl}/getAll");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<InvoiceModel>>
                {
                    IsSuccess = false,
                    Message =
                        $"Unable to load invoices. Status: {response.StatusCode}",
                    Data = new List<InvoiceModel>()
                };
            }

            return await response.Content
                .ReadFromJsonAsync<ApiResponse<List<InvoiceModel>>>()
                ?? new ApiResponse<List<InvoiceModel>>
                {
                    IsSuccess = false,
                    Message = "Invalid response from server.",
                    Data = new List<InvoiceModel>()
                };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<InvoiceModel>>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = new List<InvoiceModel>()
            };
        }
    }


    // =========================================================
    // CREATE INVOICE
    // =========================================================

    public async Task<ApiResponse<CreateInvoiceResponse>> CreateAsync(
        CreateInvoiceRequest request)
    {
        try
        {
            var response = await Client.PostAsJsonAsync(
                $"{baseUrl}/create",
                request);
            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<CreateInvoiceResponse>>();

            if (result is not null)
            {
                return result;
            }

            return new ApiResponse<CreateInvoiceResponse>
            {
                IsSuccess = false,
                Message =
                    $"Unable to create invoice. Status: {response.StatusCode}"
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


public async Task<ApiResponse<int>> UpdateAsync(
    UpdateInvoiceRequest request)
{
    var response = await Client.PutAsJsonAsync(
        $"{baseUrl}/update",
        request);

    return await response.Content
               .ReadFromJsonAsync<ApiResponse<int>>()
           ?? new ApiResponse<int>
           {
               IsSuccess = false,
               Message = "Unable to update invoice."
           };
}
}