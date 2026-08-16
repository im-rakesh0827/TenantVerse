using System.Net.Http.Json;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Unit.Requests;

namespace TenantVerse.UI.Services;

public class UnitService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public UnitService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient Client =>
        _httpClientFactory.CreateClient("TenantVerseAPI");

    private string baseUrl = "http://localhost:5168/api/Unit";

    public async Task<ApiResponse<List<UnitModel>>> GetAllAsync()
    {
        var response = await Client.GetAsync($"{baseUrl}/allUnits");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<UnitModel>>
            {
                IsSuccess = false,
                Message = $"Unable to load units. Status: {response.StatusCode}"
            };
        }

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<List<UnitModel>>>()
            ?? new ApiResponse<List<UnitModel>>
            {
                IsSuccess = false,
                Message = "Invalid response from server."
            };
    }


    public async Task<ApiResponse<List<UnitModel>>> GetByPropertyIdAsync(int propertyId)
    {
        var response = await Client.GetAsync(
            $"{baseUrl}/property/{propertyId}");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<List<UnitModel>>
            {
                IsSuccess = false,
                Message = $"Unable to load units. Status: {response.StatusCode}"
            };
        }

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<List<UnitModel>>>()
            ?? new ApiResponse<List<UnitModel>>
            {
                IsSuccess = false,
                Message = "Invalid response from server."
            };
    }


    public async Task<ApiResponse<UnitModel>> GetByIdAsync(int unitId)
    {
        var response = await Client.GetAsync(
            $"{baseUrl}/getById/{unitId}");

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<UnitModel>
            {
                IsSuccess = false,
                Message = $"Unable to load unit. Status: {response.StatusCode}"
            };
        }

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<UnitModel>>()
            ?? new ApiResponse<UnitModel>
            {
                IsSuccess = false,
                Message = "Invalid response from server."
            };
    }


    public async Task<ApiResponse<int>> CreateAsync(CreateUnitRequest request)
    {
        var response = await Client.PostAsJsonAsync(
            $"{baseUrl}/create",
            request);

        if (!response.IsSuccessStatusCode)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = $"Unable to create unit. Status: {response.StatusCode}"
            };
        }

        return await response.Content
            .ReadFromJsonAsync<ApiResponse<int>>()
            ?? new ApiResponse<int>
            {
                IsSuccess = false,
                Message = "Invalid response from server."
            };
    }


    // public async Task<ApiResponse<int>> UpdateAsync(UpdateUnitRequest request)
    // {
    //     var response = await Client.PutAsJsonAsync(
    //         $"{baseUrl}/update",
    //         request);

    //     if (!response.IsSuccessStatusCode)
    //     {
    //         return new ApiResponse<int>
    //         {
    //             IsSuccess = false,
    //             Message = $"Unable to update unit. Status: {response.StatusCode}"
    //         };
    //     }

    //     return await response.Content
    //         .ReadFromJsonAsync<ApiResponse<int>>()
    //         ?? new ApiResponse<int>
    //         {
    //             IsSuccess = false,
    //             Message = "Invalid response from server."
    //         };
    // }


    public async Task<ApiResponse<int>> UpdateAsync(UpdateUnitRequest request)
    {
        try
        {
            var response = await Client.PutAsJsonAsync(
                $"{baseUrl}/update",
                request);

            // Read the API response body even when
            // the API returns 400 BadRequest or 500.
            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<int>>();

            if (result is not null)
            {
                return result;
            }

            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = $"Unable to update unit. Status: {response.StatusCode}",
                Data = 0
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = 0
            };
        }
    }

    public async Task<ApiResponse<int>> DeleteAsync(int unitId)
    {
        try
        {
            var response = await Client.DeleteAsync(
                $"{baseUrl}/delete/{unitId}");
    
            if (!response.IsSuccessStatusCode)
            {
                return new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = $"Unable to deactivate unit. Status: {response.StatusCode}",
                    Data = 0
                };
            }
    
            var result = await response.Content
                .ReadFromJsonAsync<ApiResponse<int>>();
    
            return result ?? new ApiResponse<int>
            {
                IsSuccess = false,
                Message = "Invalid response from server.",
                Data = 0
            };
        }
        catch (Exception ex)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = ex.Message,
                Data = 0
            };
        }
    }
   
}