using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Unit.Requests;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Helpers;
namespace TenantVerse.UI.Components.Pages.Unit;

public partial class UnitViewUpdate
{
    [Parameter]
    public int Id { get; set; }
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected UnitService UnitService { get; set; } = default!;
    [Inject]
    protected PropertyService PropertyService { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected StateContainer _StateContainer { get; set; } = default!;
    protected MudForm? _form;
    protected UpdateUnitRequest _model = new();
    protected List<PropertyDto> properties = new();
    protected Dictionary<string, int> _floors = new();
    protected bool _isLoading = true;
    protected bool IsFloorDisabled { get; set; } = true;
    protected string? _errorMessage;
    protected bool IsViewMode => Navigation.Uri.Contains($"/flat/view/{Id}",StringComparison.OrdinalIgnoreCase);
    protected override async Task OnInitializedAsync()
    {
          try
          {
              _isLoading = true;
              _errorMessage = null;
              await LoadPropertiesAsync();
              await LoadUnitByIdAsync();
              await LoadFloorsAsync(_StateContainer.Unit.SelectedUnit.PropertyId);
          }
          catch (Exception ex)
          {
              _errorMessage = ex.Message;
          }
          finally
          {
              _isLoading = false;
          }
    }

    private async Task LoadPropertiesAsync()
    {

         if(_StateContainer.Property.Properties.Count()>0)
          {
               properties = _StateContainer.Property.Properties;
          }
          else
          {
               var response = await PropertyService.GetAllAsync();
               if (response == null)
               {
                    properties = new();
                    return;
               }
               properties = response.ToList();
               _StateContainer.Property.Clear();
               _StateContainer.Property.SetProperties(properties);
          }
        
    }

      private async Task LoadUnitByIdAsync()
     {
          var unit = new UnitModel();
          if(_StateContainer.Unit.SelectedUnit is not null && _StateContainer.Unit.SelectedUnit.UnitId>0)
          {
               unit = _StateContainer.Unit.SelectedUnit;
          }
          else
          {
               var response = await UnitService.GetByIdAsync(Id);
               unit = response.Data;
          }
          _model = new UpdateUnitRequest
          {
              UnitId = unit.UnitId,
              PropertyId = unit.PropertyId,
              UnitNumber = unit.UnitNumber,
              UnitType = unit.UnitType,
              FloorNumber = unit.FloorNumber,
              Bedrooms = unit.Bedrooms,
              Bathrooms = unit.Bathrooms,
              Area = unit.Area,
              MonthlyRent = unit.MonthlyRent,
              SecurityDeposit = unit.SecurityDeposit,
              Status = unit.Status
          };
     }

    protected async Task OnPropertyChanged(int propertyId)
    {
        if (IsViewMode)
            return;
        _model.PropertyId = propertyId;
        _model.FloorNumber = null;
        _floors.Clear();
        IsFloorDisabled = true;

       await LoadFloorsAsync(propertyId);
    }


    private async Task LoadFloorsAsync(int propertyId)
    {
        var selectedProperty = properties
            .FirstOrDefault(x => x.PropertyId == propertyId);
        if (selectedProperty == null)
            return;
          _floors.Clear();
          for (var i = 0; i <= selectedProperty.TotalFloors; i++)
          {
               _floors.Add(FloorHelper.GetFloorName(i), i);
          }
          IsFloorDisabled = false;
    }

    protected async Task UpdateAsync()
    {
        if (IsViewMode)
            return;
        await _form!.Validate();
        if (!_form.IsValid)
            return;
        try
        {
            _isLoading = true;
            var result = await UnitService.UpdateAsync(_model);
            if (result == null)
            {
               Snackbar.Add("Unable to update flat.",Severity.Error);
                return;
            }
            if (!result.IsSuccess)
            {
                Snackbar.Add(result.Message,Severity.Error);
                return;
            }
            await Task.Delay(500);
            _StateContainer.Unit.Clear();
            Snackbar.Add("Flat updated successfully.",Severity.Success);
            Navigation.NavigateTo("/flat");
        }
        catch (Exception ex)
        {
          Snackbar.Add(ex.Message,Severity.Error);
        }
        finally
        {
          _isLoading = false;
        }
    }

    protected void GoBack()
    {
        Navigation.NavigateTo("/flat");
    }
}