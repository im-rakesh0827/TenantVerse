using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Tenant;
using TenantVerse.UI.Services;
using TenantVerse.Shared.Helpers;
namespace TenantVerse.UI.Components.Pages.Tenant;

public partial class TenantList
{
    [Inject]
    private TenantService TenantService { get; set; } = default!;

    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    protected NavigationManager Navigation { get; set; } = default!;


    private List<TenantModel> _tenants = new();
    private string _searchString = string.Empty;
    private bool IsLoading;
    private string? _message;
    private Severity _severity = Severity.Info;
    private string _tenantStatus = "Active";


    protected IEnumerable<TenantModel> FilteredTenants =>
        string.IsNullOrWhiteSpace(_searchString)
        && string.IsNullOrWhiteSpace(_tenantStatus)
            ? _tenants
            : _tenants.Where(FilterTenant);


    protected override async Task OnInitializedAsync()
    {
        await LoadTenantsAsync();
    }


    private async Task LoadTenantsAsync()
    {
        try
        {
            IsLoading = true;
            _message = null;

            if(!_StateContainer.Tenant.IsLoaded)
            {
               await Task.Delay(1000);
               await _StateContainer.Tenant.RefreshAsync();      
          }
          _tenants = _StateContainer.Tenant.Tenants;
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }


    // private bool FilterTenant(TenantModel tenant)
    // {
    //     if (string.IsNullOrWhiteSpace(_searchString))
    //         return true;

    //     return
    //         $"{tenant.FirstName} {tenant.LastName}"
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase)

    //         || (tenant.PropertyName ?? string.Empty)
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase)

    //         || (tenant.UnitNumber ?? string.Empty)
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase)

    //         || (tenant.PhoneNumber ?? string.Empty)
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase)

    //         || (tenant.Email ?? string.Empty)
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase)

    //         || (tenant.Status ?? string.Empty)
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase)

    //         || tenant.MonthlyRent?.ToString()
    //             .Contains(
    //                 _searchString,
    //                 StringComparison.OrdinalIgnoreCase) == true;
    // }

private bool FilterTenant(TenantModel tenant)
{
    // ==========================================
    // SEARCH FILTER
    // ==========================================

    if (!string.IsNullOrWhiteSpace(_searchString))
    {
        var search = _searchString.Trim();

        var matchesSearch =
            $"{tenant.FirstName} {tenant.LastName}"
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

            || (tenant.PropertyName ?? string.Empty)
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

            || (tenant.UnitNumber ?? string.Empty)
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

            || (tenant.PhoneNumber ?? string.Empty)
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

            || (tenant.Email ?? string.Empty)
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

            || (tenant.Status ?? string.Empty)
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)

            || tenant.MonthlyRent?
                .ToString("N2")
                .Contains(
                    search,
                    StringComparison.OrdinalIgnoreCase)
                == true;

        if (!matchesSearch)
        {
            return false;
        }
    }


    // ==========================================
    // STATUS FILTER
    // ==========================================

    if (!string.IsNullOrWhiteSpace(_tenantStatus) &&
        !string.Equals(
            _tenantStatus,
            "All",
            StringComparison.OrdinalIgnoreCase))
    {
        if (!string.Equals(
                tenant.Status?.Trim(),
                _tenantStatus.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
    }


    return true;
}

    private void AddTenant()
    {
        NavigationManager.NavigateTo("/tenant/create");
    }


    private void EditOrViewTenant(int tenantId,string mode)
    {
        NavigationManager.NavigateTo($"/tenant/{mode}/{tenantId}");
    }

     private async Task DeleteTenant(int tenantId)
     {
          try
          {
              IsLoading = true;
              await Task.Delay(1000);
              var response = await TenantService.DeleteAsync(
                  tenantId,
                  "System User");

              if (!response.IsSuccess)
              {
                  Snackbar.Add(
                      response.Message,
                      Severity.Error);

                  return;
              }

              Snackbar.Add(
                  "Tenant deactivated successfully.",
                  Severity.Success);
               await _StateContainer.Unit.RefreshAsync();
              var refreshed = await _StateContainer.Tenant.RefreshAsync();
              if (!refreshed)
              {
                  Snackbar.Add(
                      "Tenant deactivated, but unable to refresh tenant list.",
                      Severity.Warning);

                  return;
              }

              _tenants = _StateContainer.Tenant.Tenants;

              await InvokeAsync(StateHasChanged);
          }
          catch (Exception ex)
          {
              Snackbar.Add(
                  ex.Message,
                  Severity.Error);
          }
          finally
          {
              IsLoading = false;
          }
     }

    private void ShowMessage()
    {
          if(!string.IsNullOrWhiteSpace(_message))
          {
               Snackbar.Add(_message, _severity);
          }
    }

    
}