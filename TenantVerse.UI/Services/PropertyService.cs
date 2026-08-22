using System.Net.Http.Json;
using TenantVerse.Shared.Models.Property;
using System.Net.Http.Headers;
using Blazored.LocalStorage;
// using TenantVerse.Shared.Models.Common;
using TenantVerse.Shared.Models;

namespace TenantVerse.UI.Services;

public class PropertyService
{
    private readonly HttpClient _httpClient;

    public PropertyService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("TenantVerseApi");
    }

    // public PropertyService(
    // IHttpClientFactory factory,
    // ILocalStorageService localStorage)
    // {
    //     _httpClient = factory.CreateClient("TenantVerseAPI");

    //     var token = localStorage.GetItemAsStringAsync("token")
    //                             .GetAwaiter()
    //                             .GetResult();

    //     if (!string.IsNullOrWhiteSpace(token))
    //     {
    //         _httpClient.DefaultRequestHeaders.Authorization =
    //             new AuthenticationHeaderValue("Bearer", token);
    //     }
    // }
    private string baseUrl = "http://localhost:5168/api/Property";

    public async Task<List<PropertyDto>> GetAllAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<PropertyDto>>>($"{baseUrl}/allProperties");

        return response?.Data ?? new List<PropertyDto>();
    }

    public async Task<PropertyDto> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetFromJsonAsync<ApiResponse<PropertyDto>>($"{baseUrl}/getById/{id}");

        return response?.Data ?? new PropertyDto();
    }


    public async Task<int> CreateAsync(CreatePropertyRequest request)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{baseUrl}/create", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Create failed: {response.StatusCode} - {error}");
                return 0;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<int>>();

            return result?.IsSuccess == true ? result.Data : 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in CreateAsync: {ex}");
            return 0;
        }
    }


    public async Task<bool> UpdateAsync(PropertyDto property)
    {
        try
        {
            Console.WriteLine($"Updating property with ID: {property.PropertyId}");
            property.UpdatedBy="TestUser";
            var response = await _httpClient.PutAsJsonAsync($"{baseUrl}/update", property);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Update failed: {response.StatusCode} - {error}");
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();

            return result?.IsSuccess ?? false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception in UpdateAsync: {ex}");
            return false;
        }
    }


    // public async Task<bool> DeleteAsync(int id, string userName)
    // {
    //     try
    //     {
    //         // Console.WriteLine("I am in DeleteAsync Method");
    //         var response = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
    //         if (!response.IsSuccessStatusCode)
    //         {
    //             var error = await response.Content.ReadAsStringAsync();
    //             Console.WriteLine(error);
    //             return false;
    //         }
    //         var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
    //         return result?.IsSuccess ?? false;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex);
    //         return false;
    //     }
    // }
    

    public async Task<ApiResponse<int>> DeleteAsync(int id, string updatedBy)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"{baseUrl}/delete/{id}");
            if (!response.IsSuccessStatusCode)
            {
                var errorResult = await response.Content
                    .ReadFromJsonAsync<ApiResponse<int>>();

                return errorResult ?? new ApiResponse<int>
                {
                    IsSuccess = false,
                    Message = $"Unable to deactivate property. Status: {response.StatusCode}",
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