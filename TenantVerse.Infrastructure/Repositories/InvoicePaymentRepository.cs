using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.Shared.Models;


namespace TenantVerse.Infrastructure.Repositories;

public class InvoicePaymentRepository : IInvoicePaymentRepository
{
    private readonly IConfiguration _configuration;

    public InvoicePaymentRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<CreateInvoicePaymentResponse?> CreateAsync(
        CreateInvoicePaymentRequest request)
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        await using var connection =
            new SqlConnection(connectionString);

        var parameters = new DynamicParameters();

        parameters.Add(
            "@InvoiceId",
            request.InvoiceId,
            DbType.Int32);

        parameters.Add(
            "@PaymentAmount",
            request.PaymentAmount,
            DbType.Decimal);

        parameters.Add(
            "@PaymentDate",
            request.PaymentDate,
            DbType.Date);

        parameters.Add(
            "@PaymentMethod",
            request.PaymentMethod,
            DbType.String);

        parameters.Add(
            "@TransactionReference",
            request.TransactionReference,
            DbType.String);

        parameters.Add(
            "@Notes",
            request.Notes,
            DbType.String);

        parameters.Add(
            "@CreatedBy",
            request.CreatedBy,
            DbType.String);

        var result =
            await connection.QuerySingleAsync<CreateInvoicePaymentResponse>(
                "dbo.IT_SP_CreateInvoicePayment",
                parameters,
                commandType: CommandType.StoredProcedure);

        return result;
    }

    public async Task<IEnumerable<InvoicePaymentModel>> GetByInvoiceIdAsync(
    int invoiceId)
{
    var connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    await using var connection =
        new SqlConnection(connectionString);

    var parameters = new DynamicParameters();

    parameters.Add(
        "@InvoiceId",
        invoiceId,
        DbType.Int32);

    var result =
        await connection.QueryAsync<InvoicePaymentModel>(
            "dbo.IT_SP_GetInvoicePaymentsById",
            parameters,
            commandType: CommandType.StoredProcedure);

    return result;
}


public async Task<ApiResponse<ReverseInvoicePaymentResponse>> ReverseAsync(
    ReverseInvoicePaymentRequest request)
{
    try
    {
        var connectionString =
            _configuration.GetConnectionString("DefaultConnection");

        await using var connection =
            new SqlConnection(connectionString);

        var parameters = new DynamicParameters();

        parameters.Add(
            "@InvoicePaymentId",
            request.InvoicePaymentId,
            DbType.Int32);

        parameters.Add(
            "@UpdatedBy",
            request.UpdatedBy,
            DbType.String);

        var result =
            await connection.QuerySingleAsync<
                ReverseInvoicePaymentResponse>(
                    "dbo.IT_SP_ReverseInvoicePayment",
                    parameters,
                    commandType: CommandType.StoredProcedure);

        return new ApiResponse<ReverseInvoicePaymentResponse>
        {
            IsSuccess = result.IsSuccess,
            Message = result.Message,
            Data = result
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<ReverseInvoicePaymentResponse>
        {
            IsSuccess = false,
            Message = ex.Message
        };
    }
}
}