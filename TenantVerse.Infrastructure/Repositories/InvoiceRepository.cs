using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Infrastructure.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly IConfiguration _configuration;

    public InvoiceRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<CreateInvoiceResponse?> CreateAsync(
        CreateInvoiceRequest request)
    {
        try
        {
            var connectionString =
                _configuration.GetConnectionString("DefaultConnection");

            await using var connection =
                new SqlConnection(connectionString);

            var parameters = new DynamicParameters();

            parameters.Add(
                "@PropertyId",
                request.PropertyId,
                DbType.Int32);

            parameters.Add(
                "@UnitId",
                request.UnitId,
                DbType.Int32);

            parameters.Add(
                "@TenantId",
                request.TenantId,
                DbType.Int32);

            parameters.Add(
                "@BillingMonth",
                request.BillingMonth,
                DbType.Date);

            parameters.Add(
                "@InvoiceDate",
                request.InvoiceDate,
                DbType.Date);

            parameters.Add(
                "@DueDate",
                request.DueDate,
                DbType.Date);

            parameters.Add(
                "@MonthlyRent",
                request.MonthlyRent,
                DbType.Decimal);

            parameters.Add(
                "@PreviousReading",
                request.PreviousReading,
                DbType.Decimal);

            parameters.Add(
                "@CurrentReading",
                request.CurrentReading,
                DbType.Decimal);

            parameters.Add(
                "@ElectricityRate",
                request.ElectricityRate,
                DbType.Decimal);

            parameters.Add(
                "@MaintenanceCharge",
                request.MaintenanceCharge,
                DbType.Decimal);

            parameters.Add(
                "@WaterCharge",
                request.WaterCharge,
                DbType.Decimal);

            parameters.Add(
                "@LateFee",
                request.LateFee,
                DbType.Decimal);

            parameters.Add(
                "@Discount",
                request.Discount,
                DbType.Decimal);

            parameters.Add(
                "@Notes",
                request.Notes,
                DbType.String);

            parameters.Add(
                "@CreatedBy",
                request.CreatedBy,
                DbType.String);

            var result =
                await connection.QuerySingleAsync<CreateInvoiceResponse>(
                    "dbo.IT_SP_CreateInvoice",
                    parameters,
                    commandType: CommandType.StoredProcedure);

            return result;
        }
        catch
        {
            throw;
        }
    }


    public async Task<IEnumerable<InvoiceModel>> GetAllAsync()
{
    var connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    await using var connection =
        new SqlConnection(connectionString);

    var result =
        await connection.QueryAsync<InvoiceModel>(
            "dbo.IT_SP_GetAllInvoice",
            commandType: CommandType.StoredProcedure);

    return result;
}

}