using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

public static class PaymentStatus
{
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
}



