namespace TenantVerse.Shared.Models.Unit;

public class UnitModel
{
    public int UnitId { get; set; }

    public int PropertyId { get; set; }

    public string? PropertyName { get; set; }

    public string UnitNumber { get; set; } = string.Empty;

    public string? UnitType { get; set; }

    public int? FloorNumber { get; set; }

    public int? Bedrooms { get; set; }

    public int? Bathrooms { get; set; }

    public decimal? Area { get; set; }

    public decimal? MonthlyRent { get; set; }

    public decimal? SecurityDeposit { get; set; }

    public string Status { get; set; } = "Available";

    public bool IsActive { get; set; }

    public DateTime CreatedDate { get; set; }

    public DateTime? ModifiedDate { get; set; }
}