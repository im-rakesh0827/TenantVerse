using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TenantVerse.Application.Interfaces.Repositories;
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


//     public async Task<IEnumerable<InvoiceModel>> GetAllAsync()
// {
//     var connectionString =
//         _configuration.GetConnectionString("DefaultConnection");

//     await using var connection =
//         new SqlConnection(connectionString);

//     var result =
//         await connection.QueryAsync<InvoiceModel>(
//             "dbo.IT_SP_GetAllInvoice",
//             commandType: CommandType.StoredProcedure);

//     return result;
// }


public async Task<IEnumerable<InvoiceModel>> GetAllAsync()
{
    var connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    await using var connection =
        new SqlConnection(connectionString);

    using var multi =
        await connection.QueryMultipleAsync(
            "dbo.IT_SP_GetAllInvoice",
            commandType: CommandType.StoredProcedure);

    var invoices =
        (await multi.ReadAsync<InvoiceModel>())
        .ToList();

    var charges =
        (await multi.ReadAsync<InvoiceChargeModel>())
        .ToList();

    var chargesByInvoice =
        charges
            .GroupBy(x => x.InvoiceId)
            .ToDictionary(
                x => x.Key,
                x => x.ToList());

    foreach (var invoice in invoices)
    {
        if (chargesByInvoice.TryGetValue(
                invoice.InvoiceId,
                out var invoiceCharges))
        {
            invoice.Charges = invoiceCharges;
        }
        else
        {
            invoice.Charges = new List<InvoiceChargeModel>();
        }
    }

    return invoices;
}

public async Task<int> UpdateAsync(
    UpdateInvoiceRequest request)
{
    try
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
            "@DiscountAmount",
            request.DiscountAmount,
            DbType.Decimal);

        parameters.Add(
            "@LateFee",
            request.LateFee,
            DbType.Decimal);

        parameters.Add(
            "@Notes",
            request.Notes,
            DbType.String);

        parameters.Add(
            "@UpdatedBy",
            request.UpdatedBy,
            DbType.String);


        // Invoice Charges
        var chargeTable = new DataTable();

        chargeTable.Columns.Add(
            "ChargeType",
            typeof(string));

        chargeTable.Columns.Add(
            "Description",
            typeof(string));

        chargeTable.Columns.Add(
            "Amount",
            typeof(decimal));

        chargeTable.Columns.Add(
            "PreviousReading",
            typeof(decimal));

        chargeTable.Columns.Add(
            "CurrentReading",
            typeof(decimal));

        chargeTable.Columns.Add(
            "Units",
            typeof(decimal));

        chargeTable.Columns.Add(
            "Rate",
            typeof(decimal));


        foreach (var charge in request.Charges)
        {
            var row = chargeTable.NewRow();

            row["ChargeType"] =
                charge.ChargeType;

            row["Description"] =
                string.IsNullOrWhiteSpace(charge.Description)
                    ? DBNull.Value
                    : charge.Description;

            row["Amount"] =
                charge.Amount;

            row["PreviousReading"] =
                charge.PreviousReading.HasValue
                    ? charge.PreviousReading.Value
                    : DBNull.Value;

            row["CurrentReading"] =
                charge.CurrentReading.HasValue
                    ? charge.CurrentReading.Value
                    : DBNull.Value;

            row["Units"] =
                charge.Units.HasValue
                    ? charge.Units.Value
                    : DBNull.Value;

            row["Rate"] =
                charge.Rate.HasValue
                    ? charge.Rate.Value
                    : DBNull.Value;

            chargeTable.Rows.Add(row);
        }


        parameters.Add(
            "@Charges",
            chargeTable.AsTableValuedParameter(
                "dbo.InvoiceChargeType"));


        var result =
            await connection.QuerySingleAsync<int>(
                "dbo.IT_SP_UpdateInvoice",
                parameters,
                commandType: CommandType.StoredProcedure);

        return result;
    }
    catch
    {
        throw;
    }
}


public async Task<InvoiceModel?> GetByIdAsync(
    int invoiceId)
{
    var connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    await using var connection =
        new SqlConnection(connectionString);

    using var multi =
        await connection.QueryMultipleAsync(
            "dbo.IT_SP_GetInvoiceById",
            new
            {
                InvoiceId = invoiceId
            },
            commandType: CommandType.StoredProcedure);

    var invoice =
        await multi.ReadFirstOrDefaultAsync<InvoiceModel>();

    if (invoice == null)
        return null;

    var charges =
        await multi.ReadAsync<InvoiceChargeModel>();

    invoice.Charges = charges.ToList();

    return invoice;
}


public async Task<IEnumerable<InvoiceChargeModel>> GetChargesByInvoiceIdAsync(
    int invoiceId)
{
    var connectionString =
        _configuration.GetConnectionString("DefaultConnection");

    await using var connection =
        new SqlConnection(connectionString);

    var charges =
        await connection.QueryAsync<InvoiceChargeModel>(
            "dbo.IT_SP_GetChargesByInvoiceId",
            new
            {
                InvoiceId = invoiceId
            },
            commandType: CommandType.StoredProcedure);

    return charges;
}


}