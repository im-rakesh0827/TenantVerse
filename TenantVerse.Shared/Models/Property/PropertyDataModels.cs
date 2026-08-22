using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.Shared.Models.Common;

namespace TenantVerse.Shared.Models.Property;

public class Property : BaseEntity
{
    public int PropertyId { get; set; }

    public string PropertyCode { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int TotalFloors { get; set; }

    public int TotalFlats { get; set; }

    public string? Description { get; set; }

}


public class CreatePropertyRequest
{
    public string PropertyCode { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = "India";

    public int TotalFloors { get; set; }

    public int TotalFlats { get; set; }

    public string? Description { get; set; }
}



public class PropertyListResponse : BaseEntity
{
    public int PropertyId { get; set; }

    public string PropertyCode { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int TotalFloors { get; set; }

    public int TotalFlats { get; set; }

    public string? Description { get; set; }
}



public class PropertyResponse : BaseEntity
{
    public int PropertyId { get; set; }

    public string PropertyCode { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int TotalFloors { get; set; }

    public int TotalFlats { get; set; }

    public string? Description { get; set; }

    // public bool IsActive { get; set; }

    // public DateTime CreatedOn { get; set; }
}



public class UpdatePropertyRequest
{
    public int PropertyId { get; set; }

    public string PropertyCode { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string AddressLine1 { get; set; } = string.Empty;

    public string? AddressLine2 { get; set; }

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = "India";

    public int TotalFloors { get; set; }

    public int TotalFlats { get; set; }

    public string? Description { get; set; }

    public bool IsActive { get; set; }
    public string UpdatedBy { get; set; } = "System";
}



public class PropertyDto : BaseEntity
{
    public int PropertyId { get; set; }

    public string PropertyCode { get; set; } = string.Empty;

    public string PropertyName { get; set; } = string.Empty;

    public string OwnerName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string AddressLine1 { get; set; } = string.Empty;

    public string AddressLine2 { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string State { get; set; } = string.Empty;

    public string PostalCode { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int TotalFloors { get; set; }

    public int TotalFlats { get; set; }

    public string Description { get; set; } = string.Empty;
}




// public class CreatePropertyRequest
// {
//     public string PropertyCode { get; set; } = string.Empty;

//     public string PropertyName { get; set; } = string.Empty;

//     public string OwnerName { get; set; } = string.Empty;

//     public string Email { get; set; } = string.Empty;

//     public string PhoneNumber { get; set; } = string.Empty;

//     public string AddressLine1 { get; set; } = string.Empty;

//     public string AddressLine2 { get; set; } = string.Empty;

//     public string City { get; set; } = string.Empty;

//     public string State { get; set; } = string.Empty;

//     public string PostalCode { get; set; } = string.Empty;

//     public string Country { get; set; } = string.Empty;

//     public int TotalFloors { get; set; }

//     public int TotalFlats { get; set; }

//     public string Description { get; set; } = string.Empty;

//     public bool IsActive { get; set; } = true;

//     public string CreatedBy { get; set; } = "Admin";
// }