using System.ComponentModel.DataAnnotations;

namespace TenantVerse.Shared.Models.Tenant;

public class CreateTenantRequest
{
    [Required]
    public int PropertyId { get; set; }

    [Required]
    public int UnitId { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [StringLength(100)]
    public string? LastName { get; set; }

    [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
    [StringLength(200)]
    public string? Email { get; set; }

    [StringLength(20)]
    public string? PhoneNumber { get; set; }

    [StringLength(150)]
    public string? EmergencyContactName { get; set; }

    [StringLength(20)]
    public string? EmergencyContactPhone { get; set; }

    public DateTime? LeaseStartDate { get; set; }

    public DateTime? LeaseEndDate { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Monthly rent cannot be negative.")]
    public decimal? MonthlyRent { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Security deposit cannot be negative.")]
    public decimal? SecurityDeposit { get; set; }

    public string Status { get; set; } = "Active";
}