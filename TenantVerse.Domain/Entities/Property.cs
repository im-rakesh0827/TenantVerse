using TenantVerse.Domain.Common;
namespace TenantVerse.Domain.Entities;


using System;

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