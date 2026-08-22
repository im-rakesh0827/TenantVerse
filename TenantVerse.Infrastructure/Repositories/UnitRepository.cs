using System.Data;
using Dapper;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Infrastructure.Persistence;

namespace TenantVerse.Infrastructure.Repositories;

public class UnitRepository : IUnitRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public UnitRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    #region Get All Units

    public async Task<List<UnitModel>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryAsync<UnitModel>(
            "IT_SP_GetAllUnit",
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    #endregion


    #region Get Unit By Id

    public async Task<UnitModel?> GetByIdAsync(int unitId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@UnitId", unitId);

        return await connection.QueryFirstOrDefaultAsync<UnitModel>(
            "IT_SP_GetUnitById",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    #endregion


    #region Get Units By Property

    public async Task<List<UnitModel>> GetByPropertyIdAsync(int propertyId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@PropertyId", propertyId);

        var result = await connection.QueryAsync<UnitModel>(
            "IT_SP_GetUnitByPropertyId",
            parameters,
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    #endregion


    #region Create Unit

    public async Task<int> CreateAsync(
        CreateUnitRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@PropertyId", request.PropertyId);
        parameters.Add("@UnitNumber", request.UnitNumber);
        parameters.Add("@UnitType", request.UnitType);
        parameters.Add("@FloorNumber", request.FloorNumber);
        parameters.Add("@Bedrooms", request.Bedrooms);
        parameters.Add("@Bathrooms", request.Bathrooms);
        parameters.Add("@Area", request.Area);
        parameters.Add("@MonthlyRent", request.MonthlyRent);
        parameters.Add("@SecurityDeposit", request.SecurityDeposit);
        parameters.Add("@Status", request.Status);

        return await connection.ExecuteScalarAsync<int>(
            "IT_SP_CreateUnit",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    #endregion


    #region Update Unit

    public async Task<int> UpdateAsync(UpdateUnitRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();
        var parameters = new DynamicParameters();

        parameters.Add("@UnitId", request.UnitId);
        parameters.Add("@PropertyId", request.PropertyId);
        parameters.Add("@UnitNumber", request.UnitNumber);
        parameters.Add("@UnitType", request.UnitType);
        parameters.Add("@FloorNumber", request.FloorNumber);
        parameters.Add("@Bedrooms", request.Bedrooms);
        parameters.Add("@Bathrooms", request.Bathrooms);
        parameters.Add("@Area", request.Area);
        parameters.Add("@MonthlyRent", request.MonthlyRent);
        parameters.Add("@SecurityDeposit", request.SecurityDeposit);
        parameters.Add("@Status", request.Status);

        return await connection.ExecuteScalarAsync<int>(
            "IT_SP_UpdateUnit",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    #endregion


    #region Delete / Deactivate Unit
    public async Task<int> DeleteAsync(int unitId, string updatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@UnitId", unitId);
        parameters.Add("@UpdatedBy", updatedBy);
        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "dbo.IT_SP_DeleteUnit",
            parameters,
            commandType: CommandType.StoredProcedure);

        return rowsAffected;
    }

    #endregion
}