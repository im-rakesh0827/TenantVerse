using TenantVerse.Application.DTOs.Property;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Domain.Entities;
using TenantVerse.Application.Common.Mappings;
using TenantVerse.Shared.Models;
namespace TenantVerse.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }

    public async Task<int> CreateAsync(CreatePropertyRequest request)
    {
        var property = PropertyMapper.ToEntity(request);

        return await _propertyRepository.CreateAsync(property);
    }

    public async Task<IEnumerable<PropertyListResponse>> GetAllAsync()
    {
        var properties = await _propertyRepository.GetAllAsync();

        return properties.Select(PropertyMapper.ToListResponse);
    }

    public async Task<PropertyResponse?> GetByIdAsync(int propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        if (property == null)
            return null;

        return PropertyMapper.ToResponse(property);
    }

    public async Task<bool> UpdateAsync(UpdatePropertyRequest request)
    {
        var existing = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (existing == null)
            return false;
        var property = PropertyMapper.ToEntity(request);
        return await _propertyRepository.UpdateAsync(property);
    }

    // public async Task<bool> DeleteAsync(int propertyId, string updatedBy)
    // {
    //     var property = await _propertyRepository.GetByIdAsync(propertyId);
    //     if (property == null)
    //         return false;
    //     return await _propertyRepository.DeleteAsync(propertyId, updatedBy);
    // }



    public async Task<ApiResponse<int>> DeleteAsync(
    int propertyId,
    string updatedBy)
{
    try
    {
        if (propertyId <= 0)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = "Invalid property ID.",
                Data = 0
            };
        }

        var rowsAffected = await _propertyRepository.DeleteAsync(
            propertyId,
            updatedBy);

        if (rowsAffected <= 0)
        {
            return new ApiResponse<int>
            {
                IsSuccess = false,
                Message = "Property not found or already inactive.",
                Data = 0
            };
        }

        return new ApiResponse<int>
        {
            IsSuccess = true,
            Message = "Property deactivated successfully.",
            Data = rowsAffected
        };
    }
    catch (Exception ex)
    {
        return new ApiResponse<int>
        {
            IsSuccess = false,
            Message = ex.Message,
            Data = 0
        };
    }
}

}