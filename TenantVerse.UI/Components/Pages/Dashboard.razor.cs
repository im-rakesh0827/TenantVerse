using Microsoft.AspNetCore.Components;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using Blazored.LocalStorage;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Tenant;

namespace TenantVerse.UI.Components.Pages;

public partial class Dashboard
{
    [Inject]
    protected PropertyService PropertyService { get; set; } = default!;
    [Inject]
    protected StateContainer _StateContainer {get; set;} = default;
    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected UnitService UnitService { get; set; } = default!;
    [Inject]
    protected TenantService TenantService { get; set; } = default!;
    private List<TenantModel> _tenants = new();

    private bool IsLoading = true;

    private int TotalProperties;
    private int TotalFlats;
    private int ActiveTenants;
    private int TotalActiveFlats{get; set;} = 0;
    private int TotalPropertiesCount{get; set;} = 0;
    private decimal PendingPayments;
    private List<PropertyDto> _properties = new();
    private List<UnitModel> _units = new();
    #region Lifecycle Methods
    // protected override async Task OnInitializedAsync()
    // {
    //     try
    //     {
    //         IsLoading = true;
    //            await Task.Delay(1000);
    //         _properties = await PropertyService.GetAllAsync();
    //         TotalProperties = _properties.Count;
    //         TotalFlats = _properties.Sum(x => x.TotalFlats);
    //         ActiveTenants = 0;
    //         PendingPayments = 0;
    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine(ex.Message);
    //     }
    //     finally
    //     {
    //         IsLoading = false;
    //     }
    // }
    #endregion

    protected override async Task OnInitializedAsync()
    {
        await LoadDashboardData();
    }
    private async Task LoadDashboardData(){
        try{
            IsLoading = true;
            if (!_StateContainer.Property.IsLoaded)
            {
                await Task.Delay(1000);
                await _StateContainer.Property.RefreshAsync();
            }

            if (!_StateContainer.Unit.IsLoaded)
            {
                await _StateContainer.Unit.RefreshAsync();
            }

            if (!_StateContainer.Tenant.IsLoaded)
            {
                await _StateContainer.Tenant.RefreshAsync();
            }
             var _allPropertiesList = _StateContainer.Property.Properties;
            TotalProperties = _allPropertiesList.Count();
            TotalFlats = _allPropertiesList.Sum(x => x.TotalFlats);
            _properties = _allPropertiesList.Take(5).ToList();
            
            
            var _allUnitList = _StateContainer.Unit.Units;
            _units = _allUnitList.Take(5).ToList();
            TotalActiveFlats = _allUnitList.Count();
           
            var _allTenants = _StateContainer.Tenant.Tenants;
            _tenants = _allTenants.Take(5).ToList();
            ActiveTenants = _allTenants.Count();
        }
        catch(Exception ex){
            throw;
        }
        finally{
            IsLoading = false;
        }
    }
}