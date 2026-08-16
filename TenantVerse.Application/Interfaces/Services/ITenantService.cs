using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Tenant;

namespace TenantVerse.Application.Interfaces.Services;

public interface ITenantService
{
    Task<ApiResponse<List<TenantModel>>> GetAllAsync();

    Task<ApiResponse<TenantModel>> GetByIdAsync(int tenantId);

    Task<ApiResponse<List<TenantModel>>> GetByPropertyIdAsync(int propertyId);

    Task<ApiResponse<List<TenantModel>>> GetByUnitIdAsync(int unitId);

    Task<ApiResponse<int>> CreateAsync(CreateTenantRequest request);

    Task<ApiResponse<int>> UpdateAsync(UpdateTenantRequest request);

    Task<ApiResponse<int>> DeleteAsync(int tenantId, string updatedBy);
}