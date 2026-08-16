namespace TenantVerse.Shared.Models.Tenant;

public class TenantModel
{
    public int TenantId { get; set; }

    public int PropertyId { get; set; }

    public int UnitId { get; set; }

    public string? PropertyName { get; set; }

    public string? UnitNumber { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? EmergencyContactName { get; set; }

    public string? EmergencyContactPhone { get; set; }

    public DateTime? LeaseStartDate { get; set; }

    public DateTime? LeaseEndDate { get; set; }

    public decimal? MonthlyRent { get; set; }

    public decimal? SecurityDeposit { get; set; }

    public string Status { get; set; } = "Active";

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}