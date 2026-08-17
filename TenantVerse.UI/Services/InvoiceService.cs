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

    public async Task<ApiResponse<List<InvoiceListModel>>> GetAllAsync()
    {
        try
        {
            var response = await Client.GetAsync(
                $"{baseUrl}/all");

            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<List<InvoiceListModel>>
                {
                    IsSuccess = false,
                    Message =
                        $"Unable to load invoices. Status: {response.StatusCode}",
                    Data = new List<InvoiceListModel>()
                };
            }

            return await response.Content
                .ReadFromJsonAsync<ApiResponse<List<InvoiceListModel>>>()
                ?? new ApiResponse<List<InvoiceListModel>>
                {
                    IsSuccess = false,
                    Message = "Invalid response from server.",
                    Data = new List<InvoiceListModel>()
                };
        }
        catch (Exception ex)
        {
            return new ApiResponse<List<InvoiceListModel>>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = new List<InvoiceListModel>()
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
}