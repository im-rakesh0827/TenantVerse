using TenantVerse.Shared.Models.Unit;

namespace TenantVerse.Shared.Models.Unit.Responses;

public class UnitResponse
{
    public UnitModel? Unit { get; set; }

    public List<UnitModel> Units { get; set; } = new();
}