using TenantVerse.Shared.Models.Property;
using TenantVerse.Shared.Models;
namespace TenantVerse.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<int> CreateAsync(CreatePropertyRequest request);

    Task<IEnumerable<PropertyListResponse>> GetAllAsync();

    Task<PropertyResponse?> GetByIdAsync(int propertyId);

    Task<bool> UpdateAsync(UpdatePropertyRequest request);

    // Task<bool> DeleteAsync(int propertyId, string updatedBy);
    Task<ApiResponse<int>> DeleteAsync(int propertyId, string updatedBy);
}

