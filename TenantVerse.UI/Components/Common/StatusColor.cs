using MudBlazor;

namespace TenantVerse.UI.Components.Common;

public static class StatusColor
{
    public static Color GetStatusColor(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "paid" => Color.Success,
            "completed" => Color.Success,

            "partially paid" => Color.Info,
            "partiallypaid" => Color.Info,

            "pending" => Color.Warning,

            "overdue" => Color.Error,
            "cancelled" => Color.Error,


            "available" => Color.Success,

            "occupied" => Color.Error,

            "maintenance" => Color.Warning,

            "active" => Color.Success,
            "inactive" => Color.Error,
    
            _ => Color.Default
        };
    }
}

