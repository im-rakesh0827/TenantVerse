using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Tenant;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.UI.Components.Constants;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;

namespace TenantVerse.UI.Components.Pages.Tenant;

public partial class TenantCreate
{
    [Inject]
    private PropertyService PropertyService { get; set; } = default!;
    [Inject]
    private UnitService UnitService { get; set; } = default!;
    [Inject]
    private TenantService TenantService { get; set; } = default!;
    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;
    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    private MudForm? _form;
    private CreateTenantRequest _model = new()
    {
        Status = "Active"
    };
    private List<PropertyDto> _properties = new();
    private List<UnitModel> _availableUnits = new();

    protected bool IsLoading;
    protected bool IsLoadingFlats => _isLoadingFlats;
    private bool _isLoadingProperties;
    private bool _isLoadingFlats;
    private string? _message;
    private Severity _severity = Severity.Info;

    
    protected override async Task OnInitializedAsync()
    {
        try
        {
          await LoadPropertiesAsync();
        }
        catch (System.Exception)
        { 
          throw;
        }
    }

    
    private async Task LoadPropertiesAsync()
    { 
          try
          {
               IsLoading = true;
               if (!_StateContainer.Property.IsLoaded)
               {
                    await Task.Delay(1000);
                    // var response = await PropertyService.GetAllAsync();
                    // _StateContainer.Property.SetProperties(response);
                    await _StateContainer.Property.RefreshAsync();
               }
               _properties = _StateContainer.Property.Properties;
          }
          catch (System.Exception)
          {
               throw;
          }
          finally
          {       
               IsLoading = false;
          }
    }

     private string GetUnitDisplayName(int unitId)
          {
          if (IsLoadingFlats)
               return "Loading flats...";

          if (!_availableUnits.Any())
               return "No flat available";

          if (unitId == 0)
               return "Select Flat";

          var unit = _availableUnits
               .FirstOrDefault(x => x.UnitId == unitId);

          return unit?.UnitNumber ?? "Select Flat";
     }

    private string GetPropertyName(int propertyId)
    {
          if (propertyId == 0)
              return "Select Property";

          var property = _properties
              .FirstOrDefault(x => x.PropertyId == propertyId);

          return property?.PropertyName ?? "Select Property";
     }

    private async Task OnPropertyChanged(int propertyId)
    {
        try
        {
               _model.PropertyId = propertyId;
               _model.UnitId = 0;
               _model.MonthlyRent = null;
               _model.SecurityDeposit = null;
               _availableUnits.Clear();
               if (propertyId <= 0)
               {
                 return;
               }
               await LoadAvailableUnitsAsync(propertyId);
        }
        catch (System.Exception)
        {
          throw;
        }
    }

    private async Task LoadAvailableUnitsAsync(int propertyId)
    {
         try
         {
              _isLoadingFlats = true;
              _message = null;
              if(!_StateContainer.Unit.IsLoaded)
              {
                    await Task.Delay(500);
                    var response = await UnitService.GetByPropertyIdAsync(propertyId);
                    if (response.IsSuccess && response.Data is not null)
                    {
                        _availableUnits = response.Data
                            .Where(x =>
                                x.IsActive &&
                                string.Equals(
                                    x.Status,
                                    "Available",
                                    StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }
                    else
                    {
                       _message = response.Message ?? "Unable to load available flats.";
                    }
               }
               else
               {
                    var result = _StateContainer.Unit.Units;
                    _availableUnits = result.Where(x => 
                                x.PropertyId==propertyId &&
                                x.IsActive &&
                                string.Equals(x.Status,"Available",StringComparison.OrdinalIgnoreCase)).ToList();
               }
          }
          catch (Exception ex)
          {
              _message = ex.Message;
          }
          finally
          {
              _isLoadingFlats = false;
          }
    }


    private Task OnUnitChanged(int unitId)
    {
        try
        {
               _model.UnitId = unitId;
               _StateContainer.Unit.SetUnitId(unitId);
               var selectedUnit = _availableUnits.FirstOrDefault(x => x.UnitId == unitId);
               if (selectedUnit is not null)
               {
                 _model.MonthlyRent = selectedUnit.MonthlyRent;
                 _model.SecurityDeposit = selectedUnit.SecurityDeposit;
                 _StateContainer.Unit.SetUnitId(_model.UnitId);
                 _StateContainer.Unit.SetSelectedUnit(selectedUnit);
               }
               else
               {
                 _model.MonthlyRent = null;
               }
               return Task.CompletedTask;
        }
        catch (System.Exception)
        {
          throw;
        }
    }


    private async Task SaveAsync()
    {
         try
         {
             IsLoading = true;
             _message = null;
             _severity = Severity.Info;

             if (_form is not null)
             {
                 await _form.Validate();

                 if (!_form.IsValid)
                 {
                     _message = "Please correct the validation errors.";
                     _severity = Severity.Warning;
                     return;
                 }
             }

             if (_model.PropertyId <= 0)
             {
                 _message = "Please select a property.";
                 _severity = Severity.Warning;
                 return;
             }

             if (_model.UnitId <= 0)
             {
                 _message = "Please select a flat.";
                 _severity = Severity.Warning;
                 return;
             }

             await Task.Delay(1000);
             var response = await TenantService.CreateAsync(_model);

             if (response is null)
             {
                 _message = "Unable to create tenant.";
                 _severity = Severity.Error;
                 return;
             }

             if (!response.IsSuccess)
             {
                 _message = response.Message ?? "Unable to create tenant.";
                 _severity = Severity.Error;
                 return;
             }
             _message = "Tenant created successfully.";
             _severity = Severity.Success;
             await _StateContainer.Unit.RefreshAsync();
             await _StateContainer.Tenant.RefreshAsync();
             NavigationManager.NavigateTo("/tenant");
         }
         catch (Exception ex)
         {
             _message = ex.Message;
             _severity = Severity.Error;
         }
         finally
         {
             IsLoading = false;

             ShowMessage();
         }
     }

    private void GoBack()
    {
        NavigationManager.NavigateTo("/tenant");
    }

    private void GetFlatDetails()
    {
          string mode = "view";
           NavigationManager.NavigateTo($"/flat/{mode}/{_StateContainer.Unit.UnitId}");  
     }

     private void ShowMessage()
    {
          if(!string.IsNullOrWhiteSpace(_message))
          {
               Snackbar.Add(_message, _severity);
          }
      
    }
}