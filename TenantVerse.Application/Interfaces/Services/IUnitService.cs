using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit;

namespace TenantVerse.Application.Interfaces.Services;

public interface IUnitService
{
    Task<ApiResponse<List<UnitModel>>> GetAllAsync();
    Task<ApiResponse<UnitModel>> GetByIdAsync(int unitId);
    Task<ApiResponse<List<UnitModel>>> GetByPropertyIdAsync(int propertyId);
    Task<ApiResponse<int>> CreateAsync(CreateUnitRequest request);
    Task<ApiResponse<int>> UpdateAsync(UpdateUnitRequest request);
    Task<ApiResponse<int>> DeleteAsync(int unitId, string updatedBy);
}