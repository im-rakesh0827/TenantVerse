using TenantVerse.Domain.Entities;

namespace TenantVerse.Application.Interfaces.Repositories;

public interface IPropertyRepository
{
    Task<int> CreateAsync(Property property);

    Task<IEnumerable<Property>> GetAllAsync();

    Task<Property?> GetByIdAsync(int propertyId);

    Task<bool> UpdateAsync(Property property);

    Task<bool> DeleteAsync(int propertyId, string updatedBy);
}