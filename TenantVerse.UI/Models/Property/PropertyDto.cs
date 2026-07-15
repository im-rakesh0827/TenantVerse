using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.Shared.Models.Common;
namespace TenantVerse.UI.Models.Property;

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