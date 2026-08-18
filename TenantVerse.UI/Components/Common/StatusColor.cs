using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MudBlazor;

namespace TenantVerse.UI.Components.Common
{
    public static class StatusColor
    {

        public  static Color GetStatusColor(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "paid" => Color.Success,
            "completed" => Color.Success,

            "partially paid" => Color.Info,
            "partiallypaid" => Color.Info,

            "pending" => Color.Warning,

            "overdue" => Color.Error,
            "cancelled" => Color.Error,

            _ => Color.Default
        };
    }
        
    }
}