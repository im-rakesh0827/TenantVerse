using TenantVerse.Shared.Models.Tenant;
namespace TenantVerse.Application.Interfaces.Repositories;

public interface ITenantRepository
{
    Task<List<TenantModel>> GetAllAsync();

    Task<TenantModel?> GetByIdAsync(int tenantId);

    Task<List<TenantModel>> GetByPropertyIdAsync(int propertyId);

    Task<List<TenantModel>> GetByUnitIdAsync(int unitId);

    Task<int> CreateAsync(CreateTenantRequest request);

    Task<int> UpdateAsync(UpdateTenantRequest request);

    Task<int> DeleteAsync(int tenantId, string updatedBy);
}