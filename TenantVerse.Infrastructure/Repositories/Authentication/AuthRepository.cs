using System.Data;
using Dapper;
using TenantVerse.Application.Interfaces.Authentication;
using TenantVerse.Infrastructure.Persistence;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.Shared.Models.Authentication.Responses;
using TenantVerse.Shared.Models.Authentication;
namespace TenantVerse.Infrastructure.Repositories.Authentication;

public class AuthRepository : IAuthRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public AuthRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<ApiResponse<RegisterResponse>> RegisterAsync(RegisterRequest request)
    {
        using IDbConnection connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@FirstName", request.FirstName);
        parameters.Add("@LastName", request.LastName);
        parameters.Add("@Email", request.Email);
        parameters.Add("@PhoneNumber", request.PhoneNumber);

        // The service will pass the hashed password
        parameters.Add("@PasswordHash", request.Password);

        // var result = await connection.QueryFirstOrDefaultAsync<RegisterResponse>(
        //     "IT_SP_RegisterUser",
        //     parameters,
        //     commandType: CommandType.StoredProcedure);

        var result = await connection.QueryFirstOrDefaultAsync<RegisterResponse>(
    "IT_SP_RegisterUser",
    parameters,
    commandType: CommandType.StoredProcedure);

        if (result == null)
        {
            return new ApiResponse<RegisterResponse>
            {
                IsSuccess = false,
                Message = "Registration failed."
            };
        }

        return new ApiResponse<RegisterResponse>
        {
            IsSuccess = true,
            Message = "User registered successfully.",
            Data = result
        };
    }

    public async Task<UserLoginData?> GetUserByEmailAsync(string email)
{
    using IDbConnection connection = _connectionFactory.CreateConnection();

    var parameters = new DynamicParameters();
    parameters.Add("@Email", email);

    var user = await connection.QueryFirstOrDefaultAsync<UserLoginData>(
        "IT_SP_GetUserByEmail",
        parameters,
        commandType: CommandType.StoredProcedure);

    return user;
}
}