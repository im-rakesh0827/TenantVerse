using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit.Requests;
using TenantVerse.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.UI.Models.Property;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Helpers;

namespace TenantVerse.UI.Components.Pages.Unit;

public partial class UnitAdd
{
    [Inject]
    protected UnitService UnitService { get; set; } = default!;
    [Inject]
    protected PropertyService PropertyService { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected HttpClient Http { get; set; } = default!;

    [Inject]
    protected StateContainer _StateContainer{get; set;} = default;
    protected List<PropertyDto> properties = new();
    protected bool _isLoading = true;
    protected int TotalPropertyCount {get; set;} = 0;
    protected string SearchString = string.Empty;

    protected MudForm? _form;
    protected CreateUnitRequest _model = new()
    {
        Status = "Available"
    };

    protected bool _isSaving;
    protected string? _errorMessage;
    protected override async Task OnInitializedAsync()
    {
        await LoadProperties();
        StateHasChanged();
    }

    private async Task LoadProperties()
    {
        _isLoading = true;
        if (!_StateContainer.Property.IsLoaded)
        {
            await Task.Delay(500);
            var data = await PropertyService.GetAllAsync();
            _StateContainer.Property.SetProperties(data);
        }
        properties = _StateContainer.Property.Properties;
        TotalPropertyCount =properties.Count();
        _isLoading = false;
    }

    protected async Task SaveAsync()
    {
        if (_form == null)
            return;

        await _form.Validate();

        if (!_form.IsValid)
            return;

        if (_model.PropertyId <= 0)
        {
            Snackbar.Add(
                "Please select a property.",
                Severity.Warning);

            return;
        }

        _isSaving = true;

        try
        {
            var result = await UnitService.GetByPropertyIdAsync(_model.PropertyId);
            if(result.Data.Count() >= _totalFlatsCount)
            {
                Snackbar.Add("All flats have been created for this property.", Severity.Warning);
                return;
            }
            var response = await UnitService.CreateAsync(_model);
            if (response == null)
            {
                Snackbar.Add("No response received from server.",Severity.Error);
                return;
            }

            if (!response.IsSuccess)
            {
                Snackbar.Add(
                    string.IsNullOrWhiteSpace(response.Message)
                        ? "Unable to create flat."
                        : response.Message,
                    Severity.Error);

                return;
            }
            Snackbar.Add("Flat created successfully.",Severity.Success);
            _StateContainer.Unit.ResetLoaded();
            Navigation.NavigateTo("/flat");
        }
        catch (HttpRequestException)
        {
            Snackbar.Add(
                "Unable to connect to TenantVerse API.",
                Severity.Error);
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                ex.Message,
                Severity.Error);
        }
        finally
        {
            _isSaving = false;
        }
    }


    protected void GoBack()
    {
        Navigation.NavigateTo("/flat");
    }

    protected int _totalFlatsCount, _totalFloorsCount;
    protected bool IsFloorDisabled{get; set;} = true;
    protected Dictionary<string, int> _floors = new();

    

    protected async Task OnPropertyChanged(int propertyId)
    {
        _model.PropertyId = propertyId;
        _totalFlatsCount = 0;
        _totalFloorsCount = 0;
        var selectedProperty = properties.FirstOrDefault(x => x.PropertyId == propertyId);
        if (selectedProperty == null) return;
        IsFloorDisabled = false;
        _totalFloorsCount = selectedProperty.TotalFloors;
        _totalFlatsCount = selectedProperty.TotalFlats;
        await LoadFloorsAsync(_totalFloorsCount);
    }

    private async Task LoadFloorsAsync(int totalFloors)
    {
          _floors.Clear();
          for (var i = 0; i <= totalFloors; i++)
          {
               _floors.Add(FloorHelper.GetFloorName(i), i);
          }
          IsFloorDisabled = false;
    }
}