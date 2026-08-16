namespace TenantVerse.UI.Components.Constants;

public static class TenantStatus
{
    public const string Active = "Active";
    public const string Inactive = "Inactive";
    public const string Pending = "Pending";

    public static readonly List<string> StatusList =
    [
        Active,
        Inactive,
        Pending
    ];
}