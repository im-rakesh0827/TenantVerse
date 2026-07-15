using TenantVerse.Application.DTOs.Property;
using TenantVerse.Application.Interfaces.Repositories;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Domain.Entities;
using TenantVerse.Application.Common.Mappings;

namespace TenantVerse.Application.Services;

public class PropertyService : IPropertyService
{
    private readonly IPropertyRepository _propertyRepository;

    public PropertyService(IPropertyRepository propertyRepository)
    {
        _propertyRepository = propertyRepository;
    }


    /*
    public async Task<int> CreateAsync(CreatePropertyRequest request)
    {
        var property = new Property
        {
            PropertyCode = request.PropertyCode,
            PropertyName = request.PropertyName,
            OwnerName = request.OwnerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            TotalFloors = request.TotalFloors,
            TotalFlats = request.TotalFlats,
            Description = request.Description,
            IsActive = true,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = "System"
        };

        return await _propertyRepository.CreateAsync(property);
    }


    

    public async Task<IEnumerable<PropertyListResponse>> GetAllAsync()
    {
        var properties = await _propertyRepository.GetAllAsync();

        return properties.Select(p => new PropertyListResponse
        {
            PropertyId = p.PropertyId,
            PropertyCode = p.PropertyCode,
            PropertyName = p.PropertyName,
            OwnerName = p.OwnerName,
            City = p.City,
            State = p.State,
            TotalFlats = p.TotalFlats,
            IsActive = p.IsActive
        });
    }


    public async Task<PropertyResponse?> GetByIdAsync(int propertyId)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);

        if (property == null)
            return null;

        return new PropertyResponse
        {
            PropertyId = property.PropertyId,
            PropertyCode = property.PropertyCode,
            PropertyName = property.PropertyName,
            OwnerName = property.OwnerName,
            Email = property.Email,
            PhoneNumber = property.PhoneNumber,
            AddressLine1 = property.AddressLine1,
            AddressLine2 = property.AddressLine2,
            City = property.City,
            State = property.State,
            PostalCode = property.PostalCode,
            Country = property.Country,
            TotalFloors = property.TotalFloors,
            TotalFlats = property.TotalFlats,
            Description = property.Description,
            IsActive = property.IsActive,
            CreatedOn = property.CreatedOn
        };
    }


    public async Task<bool> UpdateAsync(UpdatePropertyRequest request)
    {
        var property = new Property
        {
            PropertyId = request.PropertyId,
            PropertyCode = request.PropertyCode,
            PropertyName = request.PropertyName,
            OwnerName = request.OwnerName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            AddressLine1 = request.AddressLine1,
            AddressLine2 = request.AddressLine2,
            City = request.City,
            State = request.State,
            PostalCode = request.PostalCode,
            Country = request.Country,
            TotalFloors = request.TotalFloors,
            TotalFlats = request.TotalFlats,
            Description = request.Description,
            IsActive = request.IsActive,
            UpdatedOn = DateTime.UtcNow,
            UpdatedBy = "System"
        };

        return await _propertyRepository.UpdateAsync(property);
    }
    */



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



    // public async Task<bool> UpdateAsync(UpdatePropertyRequest request)
    // {
    //     var property = PropertyMapper.ToEntity(request);
    //     return await _propertyRepository.UpdateAsync(property);
    // }

    public async Task<bool> UpdateAsync(UpdatePropertyRequest request)
    {
        var existing = await _propertyRepository.GetByIdAsync(request.PropertyId);
        if (existing == null)
            return false;
        var property = PropertyMapper.ToEntity(request);
        return await _propertyRepository.UpdateAsync(property);
    }

    public async Task<bool> DeleteAsync(int propertyId, string updatedBy)
    {
        var property = await _propertyRepository.GetByIdAsync(propertyId);
        if (property == null)
            return false;
        return await _propertyRepository.DeleteAsync(propertyId, updatedBy);
    }

}