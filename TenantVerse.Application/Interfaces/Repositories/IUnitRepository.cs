using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Unit.Requests;

namespace TenantVerse.Application.Interfaces.Repositories;

public interface IUnitRepository
{
    Task<List<UnitModel>> GetAllAsync();
    Task<UnitModel?> GetByIdAsync(int unitId);
    Task<List<UnitModel>> GetByPropertyIdAsync(int propertyId);
    Task<int> CreateAsync(CreateUnitRequest request);
    Task<int> UpdateAsync(UpdateUnitRequest request);
    Task<int> DeleteAsync(int unitId,string updatedBy);
}