using Microsoft.AspNetCore.Components;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using Blazored.LocalStorage;
using TenantVerse.Shared.Models.Unit;
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
    
    private bool IsLoading = true;

    private int TotalProperties;
    private int TotalFlats;
    private int ActiveTenants;
    private int TotalActiveFlats{get; set;} = 100;
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
                var response = await PropertyService.GetAllAsync();
                _StateContainer.Property.SetProperties(response);
            }
            if(!_StateContainer.Unit.IsLoaded)
            {
                var response = await UnitService.GetAllAsync();
                _StateContainer.Unit.SetUnits(response.Data.ToList());
            }
            var _allUnitList = _StateContainer.Unit.Units;
            _units = _allUnitList.Take(5).ToList();
            TotalActiveFlats = _allUnitList.Count();
            var _allPropertiesList = _StateContainer.Property.Properties;
            TotalProperties = _allPropertiesList.Count();
            TotalFlats = _allPropertiesList.Sum(x => x.TotalFlats);
            _properties = _allPropertiesList.Take(5).ToList();
        }
        catch(Exception ex){
            throw;
        }
        finally{
            IsLoading = false;
        }
    }
}