using System.Data;
using Dapper;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Domain.Entities;
using TenantVerse.Infrastructure.Persistence;
using Dapper;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Domain.Entities;
using TenantVerse.Infrastructure.Persistence;

namespace TenantVerse.Infrastructure.Repositories;

public class PropertyRepository : IPropertyRepository
{
    private readonly IDbConnectionFactory _connectionFactory;

    public PropertyRepository(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }


    #region Create Property Implementation
    // public async Task<int> CreateAsync(Property property)
    //     {
    //         using var connection = _connectionFactory.CreateConnection();

    //         const string sql = @"
    //         INSERT INTO tbl_Property
    //         (
    //             PropertyCode,
    //             PropertyName,
    //             OwnerName,
    //             Email,
    //             PhoneNumber,
    //             AddressLine1,
    //             AddressLine2,
    //             City,
    //             State,
    //             PostalCode,
    //             Country,
    //             TotalFloors,
    //             TotalFlats,
    //             Description,
    //             IsActive,
    //             CreatedOn,
    //             CreatedBy
    //         )
    //         VALUES
    //         (
    //             @PropertyCode,
    //             @PropertyName,
    //             @OwnerName,
    //             @Email,
    //             @PhoneNumber,
    //             @AddressLine1,
    //             @AddressLine2,
    //             @City,
    //             @State,
    //             @PostalCode,
    //             @Country,
    //             @TotalFloors,
    //             @TotalFlats,
    //             @Description,
    //             @IsActive,
    //             @CreatedOn,
    //             @CreatedBy
    //         );

    //         SELECT CAST(SCOPE_IDENTITY() AS INT);
    //         ";

    //         return await connection.ExecuteScalarAsync<int>(sql, property);
    //     }
#endregion
    public async Task<int> CreateAsync(Property property)
    {
        using var connection = _connectionFactory.CreateConnection();
        var parameters = new DynamicParameters();
        parameters.Add("@PropertyCode", property.PropertyCode);
        parameters.Add("@PropertyName", property.PropertyName);
        parameters.Add("@OwnerName", property.OwnerName);
        parameters.Add("@Email", property.Email);
        parameters.Add("@PhoneNumber", property.PhoneNumber);
        parameters.Add("@AddressLine1", property.AddressLine1);
        parameters.Add("@AddressLine2", property.AddressLine2);
        parameters.Add("@City", property.City);
        parameters.Add("@State", property.State);
        parameters.Add("@PostalCode", property.PostalCode);
        parameters.Add("@Country", property.Country);
        parameters.Add("@TotalFloors", property.TotalFloors);
        parameters.Add("@TotalFlats", property.TotalFlats);
        parameters.Add("@Description", property.Description);
        parameters.Add("@CreatedBy", property.CreatedBy);

        var propertyId = await connection.ExecuteScalarAsync<int>(
            "dbo.IT_SP_CreateProperty",
            parameters,
            commandType: CommandType.StoredProcedure);

        return propertyId;
    }



    #region Get All Properties Implementation
// public async Task<IEnumerable<Property>> GetAllAsync()
    // {
    //     using var connection = _connectionFactory.CreateConnection();

    //     const string sql = @"
    //         SELECT
    //             PropertyId,
    //             PropertyCode,
    //             PropertyName,
    //             OwnerName,
    //             Email,
    //             PhoneNumber,
    //             AddressLine1,
    //             AddressLine2,
    //             City,
    //             State,
    //             PostalCode,
    //             Country,
    //             TotalFloors,
    //             TotalFlats,
    //             Description,
    //             IsActive,
    //             CreatedOn,
    //             CreatedBy,
    //             UpdatedOn,
    //             UpdatedBy
    //         FROM Property
    //         WHERE IsActive = 1
    //         ORDER BY PropertyName;";

    //     return await connection.QueryAsync<Property>(sql);
    // }

    // public async Task<IEnumerable<Property>> GetAllAsync()
    // {
    //     using var connection = _connectionFactory.CreateConnection();

    //     return await connection.QueryAsync<Property>(
    //         "IT_SP_GetAllProperties",
    //         commandType: CommandType.StoredProcedure);
    // }
#endregion

    public async Task<IEnumerable<Property>> GetAllAsync()
    {
        return await QueryAsync<Property>("IT_SP_GetAllProperties");
    }

    
    #region Get Property By Id Implementation
    // public async Task<Property?> GetByIdAsync(int propertyId)
    // {
    //     using var connection = _connectionFactory.CreateConnection();

    //     const string sql = @"
    //     SELECT
    //         PropertyId,
    //         PropertyCode,
    //         PropertyName,
    //         OwnerName,
    //         Email,
    //         PhoneNumber,
    //         AddressLine1,
    //         AddressLine2,
    //         City,
    //         State,
    //         PostalCode,
    //         Country,
    //         TotalFloors,
    //         TotalFlats,
    //         Description,
    //         IsActive,
    //         CreatedOn,
    //         CreatedBy,
    //         UpdatedOn,
    //         UpdatedBy
    //     FROM Property
    //     WHERE PropertyId = @PropertyId
    //     AND IsActive = 1;";

    //     return await connection.QueryFirstOrDefaultAsync<Property>(
    //         sql,
    //         new
    //         {
    //             PropertyId = propertyId
    //         });
    // }
    #endregion
    public async Task<Property?> GetByIdAsync(int propertyId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();
        parameters.Add("@PropertyId", propertyId);

        return await connection.QueryFirstOrDefaultAsync<Property>(
            "IT_SP_GetPropertyById",
            parameters,
            commandType: CommandType.StoredProcedure);
    }

    public async Task<bool> UpdateAsync(Property property)
    {
        // Console.WriteLine("I am in UpdateAsync Method In Repo");
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@PropertyId", property.PropertyId);
        parameters.Add("@PropertyCode", property.PropertyCode);
        parameters.Add("@PropertyName", property.PropertyName);
        parameters.Add("@OwnerName", property.OwnerName);
        parameters.Add("@Email", property.Email);
        parameters.Add("@PhoneNumber", property.PhoneNumber);
        parameters.Add("@AddressLine1", property.AddressLine1);
        parameters.Add("@AddressLine2", property.AddressLine2);
        parameters.Add("@City", property.City);
        parameters.Add("@State", property.State);
        parameters.Add("@PostalCode", property.PostalCode);
        parameters.Add("@Country", property.Country);
        parameters.Add("@TotalFloors", property.TotalFloors);
        parameters.Add("@TotalFlats", property.TotalFlats);
        parameters.Add("@Description", property.Description);
        parameters.Add("@UpdatedBy", property.UpdatedBy);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
        "IT_SP_UpdateProperty",
        parameters,
        commandType: CommandType.StoredProcedure);

        return rowsAffected > 0;
    }

    // public async Task<bool> DeleteAsync(int propertyId)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<bool> DeleteAsync(int propertyId, string updatedBy)
    {
        using var connection = _connectionFactory.CreateConnection();

        var parameters = new DynamicParameters();

        parameters.Add("@PropertyId", propertyId);
        parameters.Add("@UpdatedBy", updatedBy);

        var rowsAffected = await connection.ExecuteScalarAsync<int>(
            "IT_SP_DeleteProperty",
            parameters,
            commandType: CommandType.StoredProcedure);

        return rowsAffected > 0;
    }


    protected async Task<IEnumerable<T>> QueryAsync<T>(
    string procedureName,
    object? parameters = null)
    {
        using var connection = _connectionFactory.CreateConnection();

        return await connection.QueryAsync<T>(
            procedureName,
            parameters,
            commandType: CommandType.StoredProcedure);
    }
}