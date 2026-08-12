using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
namespace TenantVerse.Shared.Helpers
{
    public static class SweetAlerts
    {

        public static async Task ShowSweetAlerts(this IJSRuntime js, string type, string title, string message, string button = "")
        {
            if (!string.IsNullOrEmpty(button))
            {
                await js.InvokeVoidAsync("ShowSweetAlert", type, title, message, button);
            }
            else
            {
                await js.InvokeVoidAsync("ShowSweetAlertWithoutButton", type, title, message);
            }
        }

        public static  ValueTask<bool> ShowConfirmSweetAlerts(this IJSRuntime js, string type, string title, string message)
        {
            return js.InvokeAsync<bool>("ShowConfirmSweetAlert", title, message);
            
        }

    }

    public static class ToastRAlerts
    {
        public static async Task ShowToastRAlerts(IJSRuntime js, string type, string title, string message, string position="toast-top-right")
        {
            await js.InvokeVoidAsync("ShowToastR", type, title, message, position);
        }
        public static async Task ShowBottomToastRAlerts(IJSRuntime js, string type, string title, string message, string position = "toast-bottom-center")
        {
            await js.InvokeVoidAsync("ShowBottomToaster", type, title, message, position);
        }
    }
}
