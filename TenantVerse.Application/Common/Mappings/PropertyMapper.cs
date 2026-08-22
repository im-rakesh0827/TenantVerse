using TenantVerse.Shared.Models.Property;


namespace TenantVerse.Application.Common.Mappings;

public static class PropertyMapper
{
    public static Property ToEntity(CreatePropertyRequest request)
    {
        return new Property
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
    }

    public static Property ToEntity(UpdatePropertyRequest request)
    {
        return new Property
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
            UpdatedBy = request.UpdatedBy,
            UpdatedOn = DateTime.UtcNow
        };
    }

    public static PropertyResponse ToResponse(Property property)
    {
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
            CreatedBy = property.CreatedBy,
            CreatedOn = property.CreatedOn,
            UpdatedBy = property.UpdatedBy,
            UpdatedOn = property.UpdatedOn
        };
    }

    public static PropertyListResponse ToListResponse(Property property)
    {
        return new PropertyListResponse
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
            CreatedBy = property.CreatedBy,
            CreatedOn = property.CreatedOn,
            UpdatedBy = property.UpdatedBy,
            UpdatedOn = property.UpdatedOn
        };
    }
}