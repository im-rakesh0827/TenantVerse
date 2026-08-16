using System.Data;
using Dapper;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Infrastructure.Persistence;
using TenantVerse.Shared.Models.Tenant;

namespace TenantVerse.Infrastructure.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TenantRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<List<TenantModel>> GetAllAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryAsync<TenantModel>(
            "dbo.IT_SP_GetAllTenant",
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<TenantModel?> GetByIdAsync(int tenantId)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryFirstOrDefaultAsync<TenantModel>(
            "dbo.IT_SP_GetTenantById",
            new
            {
                TenantId = tenantId
            },
            commandType: CommandType.StoredProcedure);
    }

    public async Task<List<TenantModel>> GetByPropertyIdAsync(int propertyId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryAsync<TenantModel>(
            "dbo.IT_SP_GetTenantByPropertyId",
            new
            {
                PropertyId = propertyId
            },
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<List<TenantModel>> GetByUnitIdAsync(int unitId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var result = await connection.QueryAsync<TenantModel>(
            "dbo.IT_SP_GetTenantByUnitId",
            new
            {
                UnitId = unitId
            },
            commandType: CommandType.StoredProcedure);

        return result.ToList();
    }

    public async Task<int> CreateAsync(CreateTenantRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            "dbo.IT_SP_CreateTenant",
            request,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> UpdateAsync(UpdateTenantRequest request)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            "dbo.IT_SP_UpdateTenant",
            request,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<int> DeleteAsync(int tenantId, string updatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QuerySingleAsync<int>(
            "dbo.IT_SP_DeleteTenant",
            new
            {
                TenantId = tenantId
            },
            commandType: CommandType.StoredProcedure);
    }
}