using TenantVerse.Shared.Models;

namespace TenantVerse.Shared.Helpers;

public static class ApiResponseHelper
{
    public static ApiResponse<T> Success<T>(
        T data,
        string message = "Request completed successfully.")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> Fail<T>(
        string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}