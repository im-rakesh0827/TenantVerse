using BCrypt.Net;
using TenantVerse.Application.Interfaces.Authentication;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;
// using TenantVerse.Infrastructure.Services.Authentication;
// using System.Security.Cryptography;

namespace TenantVerse.Application.Services.Authentication;

public class AuthService : IAuthService
{
    private readonly IAuthRepository _authRepository;
    private readonly IJwtService _jwtService;

    // public AuthService(IAuthRepository authRepository)
    // {
    //     _authRepository = authRepository;
    // }

    public AuthService(
    IAuthRepository authRepository,
    IJwtService jwtService)
{
    _authRepository = authRepository;
    _jwtService = jwtService;
}

    #region Register

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FirstName))
        {
            return new ApiResponse<RegisterResponse>
            {
                IsSuccess = false,
                Message = "First Name is required."
            };
        }

        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return new ApiResponse<RegisterResponse>
            {
                IsSuccess = false,
                Message = "Email is required."
            };
        }

        if (string.IsNullOrWhiteSpace(request.Password))
        {
            return new ApiResponse<RegisterResponse>
            {
                IsSuccess = false,
                Message = "Password is required."
            };
        }

        if (request.Password != request.ConfirmPassword)
        {
            return new ApiResponse<RegisterResponse>
            {
                IsSuccess = false,
                Message = "Password and Confirm Password do not match."
            };
        }

        request.Password = BCrypt.Net.BCrypt.HashPassword(request.Password);

        return await _authRepository.RegisterAsync(request);
    }

    #endregion

    #region Login

    public async Task<ApiResponse<LoginResponse>> LoginAsync(LoginRequest request)
{
    if (string.IsNullOrWhiteSpace(request.Email))
    {
        return new ApiResponse<LoginResponse>
        {
            IsSuccess = false,
            Message = "Email is required."
        };
    }

    if (string.IsNullOrWhiteSpace(request.Password))
    {
        return new ApiResponse<LoginResponse>
        {
            IsSuccess = false,
            Message = "Password is required."
        };
    }

    var user = await _authRepository.GetUserByEmailAsync(request.Email);

    if (user == null)
    {
        return new ApiResponse<LoginResponse>
        {
            IsSuccess = false,
            Message = "Invalid email or password."
        };
    }

    if (!user.IsActive)
    {
        return new ApiResponse<LoginResponse>
        {
            IsSuccess = false,
            Message = "Your account is inactive."
        };
    }

    if (user.IsDeleted)
    {
        return new ApiResponse<LoginResponse>
        {
            IsSuccess = false,
            Message = "Your account has been deleted."
        };
    }

    bool isPasswordValid =
        BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

    if (!isPasswordValid)
    {
        return new ApiResponse<LoginResponse>
        {
            IsSuccess = false,
            Message = "Invalid email or password."
        };
    }

    //This is for generating secret key
    // var key = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    // Console.WriteLine($"Randomly Generated Number : {key}");
    return new ApiResponse<LoginResponse>
    {
        IsSuccess = true,
        Message = "Login successful.",
        Data = new LoginResponse
        {
            UserId = user.UserId,
            UserCode = user.UserCode,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role,
            Token = _jwtService.GenerateToken(user)
        }
    };
    
}
    #endregion
}