using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;

namespace TenantVerse.UI.Components.Constants;


public static class UnitStatus
{
    // public const string Available = "Available";
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



    public const string All = "All";
    public const string Available = "Available";
    public const string Occupied = "Occupied";
    public const string Maintenance = "Maintenance";

    public static readonly List<string> FilterStatusList =
    [
        All,
        Available,
        Occupied,
        Maintenance
    ];

}


public static class TenantStatus
{
    public const string All = "All";
    public const string Active = "Active";
    public const string Inactive = "Inactive";
    public const string Pending = "Pending";

    public static readonly List<string> StatusList =
    [
        Active,
        Inactive,
        Pending
    ];

    public static readonly List<string> FilterStatusList =
    [
        All,
        Active,
        Inactive,
        Pending
    ];
}

public static class PaymentStatus
{
    public const string All = "All";
    public const string Pending = "Pending";
    public const string Paid = "Paid";
    public const string PartiallyPaid = "Partially Paid";
    public const string Overdue = "Overdue";
    public const string Cancelled = "Cancelled";


    public static readonly List<string> StatusList =
    [
        Pending,
        Paid,
        PartiallyPaid,
        Overdue,
        Cancelled
    ];


    public static readonly List<string> FilterStatusList =
    [
        All,
        Pending,
        Paid,
        PartiallyPaid,
        Overdue,
        Cancelled
    ];
}









// Status Color And Icon
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


public static class StatusIcon
{
    public static string GetStatusIcon(string? status)
{
    return status?.Trim().ToLowerInvariant() switch
    {
        "available" => Icons.Material.Filled.CheckCircle,

        "occupied" => Icons.Material.Filled.Home,

        "active" => Icons.Material.Filled.CheckCircle,

        "inactive" => Icons.Material.Filled.Cancel,

        "pending" => Icons.Material.Filled.Pending,

        "paid" => Icons.Material.Filled.CheckCircle,

        "partially paid" => Icons.Material.Filled.PieChart,

        "overdue" => Icons.Material.Filled.Warning,

        "cancelled" => Icons.Material.Filled.Cancel,

        "void" => Icons.Material.Filled.Block,

        _ => Icons.Material.Filled.Info
    };
}
}