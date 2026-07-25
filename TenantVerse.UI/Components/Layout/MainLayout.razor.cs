using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using TenantVerse.UI.Services.Authentication;

namespace TenantVerse.UI.Components.Layout
{
    public partial class MainLayout
    {
     private bool _drawerOpen = true;
    private bool _loading = true;
    private const string DrawerStateKey = "tv_drawer";

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private AuthService AuthService { get; set; } = default!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var state = await LocalStorage.GetItemAsync<bool?>(DrawerStateKey);
            _drawerOpen = state ?? true;
            _loading = false;
            StateHasChanged();
        }
    }

    private async Task ToggleDrawer()
    {
        _drawerOpen = !_drawerOpen;
        await LocalStorage.SetItemAsync(DrawerStateKey, _drawerOpen);
    }

    private async Task Logout()
    {
        await AuthService.LogoutAsync();
        NavigationManager.NavigateTo("/login", true);
    }
    }
}