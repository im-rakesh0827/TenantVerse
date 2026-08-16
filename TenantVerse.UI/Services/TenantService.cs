using System.Net.Http.Json;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Tenant;

namespace TenantVerse.UI.Services;

public class TenantService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TenantService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Client =>
        _httpClientFactory.CreateClient("TenantVerseAPI");

    private string baseUrl = "http://localhost:5168/api/Tenant";

    public async Task<ApiResponse<List<TenantModel>>> GetAllAsync()
    {
        var response =
            await Client.GetFromJsonAsync<ApiResponse<List<TenantModel>>>($"{baseUrl}/getAllTenant");

        return response ?? new ApiResponse<List<TenantModel>>
        {
            IsSuccess = false,
            Message = "Unable to retrieve tenants."
        };
    }

    public async Task<ApiResponse<TenantModel>> GetByIdAsync(int tenantId)
    {
        var response =
            await Client.GetFromJsonAsync<ApiResponse<TenantModel>>(
                $"{baseUrl}/getTenantById/{tenantId}");

        return response ?? new ApiResponse<TenantModel>
        {
            IsSuccess = false,
            Message = "Unable to retrieve tenant."
        };
    }

    public async Task<ApiResponse<List<TenantModel>>> GetByPropertyIdAsync(
        int propertyId)
    {
        var response =
            await Client.GetFromJsonAsync<ApiResponse<List<TenantModel>>>(
                $"{baseUrl}/property/{propertyId}");

        return response ?? new ApiResponse<List<TenantModel>>
        {
            IsSuccess = false,
            Message = "Unable to retrieve tenants."
        };
    }

    public async Task<ApiResponse<List<TenantModel>>> GetByUnitIdAsync(
        int unitId)
    {
        var response =
            await Client.GetFromJsonAsync<ApiResponse<List<TenantModel>>>(
                $"{baseUrl}/getByUnitId/{unitId}");

        return response ?? new ApiResponse<List<TenantModel>>
        {
            IsSuccess = false,
            Message = "Unable to retrieve tenants."
        };
    }

    public async Task<ApiResponse<int>> CreateAsync(CreateTenantRequest request)
    {
        var response = await Client.PostAsJsonAsync($"{baseUrl}/create",request);
        return await response.Content
                   .ReadFromJsonAsync<ApiResponse<int>>()
               ?? new ApiResponse<int>
               {
                   IsSuccess = false,
                   Message = "Unable to create tenant."
               };
    }

    public async Task<ApiResponse<int>> UpdateAsync(UpdateTenantRequest request)
    {
        var response = await Client.PutAsJsonAsync(
            $"{baseUrl}/update",
            request);

        return await response.Content
                   .ReadFromJsonAsync<ApiResponse<int>>()
               ?? new ApiResponse<int>
               {
                   IsSuccess = false,
                   Message = "Unable to update tenant."
               };
    }

    public async Task<ApiResponse<int>> DeleteAsync(
        int tenantId,
        string updatedBy)
    {
        var response = await Client.DeleteAsync(
            $"{baseUrl}/{tenantId}?updatedBy={Uri.EscapeDataString(updatedBy)}");

        return await response.Content
                   .ReadFromJsonAsync<ApiResponse<int>>()
               ?? new ApiResponse<int>
               {
                   IsSuccess = false,
                   Message = "Unable to delete tenant."
               };
    }
}