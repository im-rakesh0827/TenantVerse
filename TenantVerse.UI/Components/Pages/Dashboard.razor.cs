using Microsoft.AspNetCore.Components;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;


namespace TenantVerse.UI.Components.Pages;

public partial class Dashboard
{
    [Inject]
    protected PropertyService PropertyService { get; set; } = default!;

    [Inject]
    protected StateContainer _StateContainer {get; set;} = default;
    
    private bool IsLoading = true;

    private int TotalProperties;
    private int TotalFlats;
    private int ActiveTenants;
    private decimal PendingPayments;

    private List<PropertyDto> _properties = new();

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
        IsLoading = true;
        try
        {
            if (!_StateContainer.Property.IsLoaded)
            {
                Console.WriteLine("Loading properties from API...1");
                await Task.Delay(1000); 
                var data = await PropertyService.GetAllAsync();
                _StateContainer.Property.SetProperties(data);
            }
            _properties = _StateContainer.Property.Properties;
            TotalProperties = _properties.Count;
            TotalFlats = _properties.Sum(x => x.TotalFlats);
            ActiveTenants = 0;
            PendingPayments = 0;
        }
        finally
        {
            IsLoading = false;
        }
    }
}