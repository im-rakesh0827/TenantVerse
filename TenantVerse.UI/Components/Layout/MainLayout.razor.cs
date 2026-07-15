using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantVerse.UI.Components.Layout
{
    public partial class MainLayout
    {
     private bool _drawerOpen = true;
    private bool _loading = true;
    private const string DrawerStateKey = "tv_drawer";

    // protected override async Task OnAfterRenderAsync(bool firstRender)
    // {
    //     if (firstRender)
    //     {
    //         try
    //         {
    //             _drawerOpen = await JS.InvokeAsync<bool>(
    //                 "tenantVerse.getDrawerState");
    //         }
    //         catch
    //         {
    //             _drawerOpen = true;
    //         }

    //         _loading = false;

    //         StateHasChanged();
    //     }
    // }

    // private async Task ToggleDrawer()
    // {
    //     _drawerOpen = !_drawerOpen;

    //     await JS.InvokeVoidAsync(
    //         "tenantVerse.saveDrawerState",
    //         _drawerOpen);
    // }




    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // var state = await LocalStorage.GetItemAsync<bool?>("tv_drawer");
            var state = await LocalStorage.GetItemAsync<bool?>(DrawerStateKey);

            _drawerOpen = state ?? true;

            _loading = false;

            StateHasChanged();
        }
    }

    private async Task ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;

        // await LocalStorage.SetItemAsync("tv_drawer", _drawerOpen);
        await LocalStorage.SetItemAsync(DrawerStateKey, _drawerOpen);
    }
        
        
    }
}