namespace TenantVerse.UI.Models.Property;

public class CreatePropertyRequest
{
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

    public bool IsActive { get; set; } = true;

    public string CreatedBy { get; set; } = "Admin";
}