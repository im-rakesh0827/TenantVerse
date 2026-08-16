namespace TenantVerse.UI.Components.Constants;
public static class UnitStatus
{
    public const string Available = "Available";
    // public const string Occupied = "Occupied";
    // public const string Maintenance = "Maintenance";
    // public static readonly List<string> StatusList =
    // [
    //     Available,
    //     Occupied,
    //     Maintenance
    // ];

    private static readonly string[] StatusArray =
    {
        "Available",
        "Occupied",
        "Maintenance"
    };

    public static readonly List<string> StatusList = new();
    static UnitStatus()
    {
        foreach (var status in StatusArray)
        {
            StatusList.Add(status);
        }
    }
}