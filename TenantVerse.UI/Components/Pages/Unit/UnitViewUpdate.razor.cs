using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Property;
using TenantVerse.UI.Services;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Helpers;
namespace TenantVerse.UI.Components.Pages.Unit;

public partial class UnitViewUpdate
{
    [Parameter]
    public int Id { get; set; }
    [Parameter]
    public string Mode { get; set; } = string.Empty;
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
    protected bool IsLoading = true;
    protected bool IsFloorDisabled { get; set; } = false;
    protected string? _errorMessage;
    protected bool IsViewMode => Navigation.Uri.Contains($"/flat/view/{Id}",StringComparison.OrdinalIgnoreCase);
    protected override async Task OnInitializedAsync()
    {
          try
          {
              IsLoading = true;
              _errorMessage = null;
              await LoadPropertiesAsync();
              await LoadUnitByIdAsync();
              await InvokeAsync(StateHasChanged);

          }
          catch (Exception ex)
          {
              _errorMessage = ex.Message;
          }
          finally
          {
              IsLoading = false;
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
               _StateContainer.Unit.SetSelectedUnit(unit);
          }
          await LoadFloorsAsync(unit.PropertyId);
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
        await LoadFloorsAsync(propertyId);
    }


    private async Task LoadFloorsAsync(int propertyId)
    {
        _floors.Clear();
         IsFloorDisabled = true;
        var selectedProperty = properties.FirstOrDefault(x => x.PropertyId == propertyId);
        if (selectedProperty == null) return;
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
            IsLoading = true;
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
           var response = await UnitService.GetAllAsync();
           _StateContainer.Unit.SetUnits(response.Data.ToList());
            await Task.Delay(1000);
            Snackbar.Add("Flat updated successfully.",Severity.Success);
            Navigation.NavigateTo("/flat");
        }
        catch (Exception ex)
        {
          Snackbar.Add(ex.Message,Severity.Error);
        }
        finally
        {
          IsLoading = false;
          await InvokeAsync(StateHasChanged);
        }
    }
    protected void GoBack()
    {
        Navigation.NavigateTo("/flat");
    }









    private Task<IEnumerable<int>> SearchProperties(
    string? value,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return Task.FromResult(
            properties.Select(x => x.PropertyId));
    }

    var result = properties
        .Where(x =>
            x.PropertyName.Contains(
                value,
                StringComparison.OrdinalIgnoreCase))
        .Select(x => x.PropertyId);

    return Task.FromResult(result);
}

private string GetPropertyName(int propertyId)
{
    return properties
        .FirstOrDefault(x => x.PropertyId == propertyId)
        ?.PropertyName ?? string.Empty;
}
}