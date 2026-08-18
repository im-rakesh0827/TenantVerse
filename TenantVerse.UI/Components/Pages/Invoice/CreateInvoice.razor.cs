using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Tenant;
using TenantVerse.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using TenantVerse.UI.Components.Shared;
namespace TenantVerse.UI.Components.Pages.Invoice;
public partial class CreateInvoice
{
    [Inject]
    private PropertyService PropertyService { get; set; } = default!;

    [Inject]
    private UnitService UnitService { get; set; } = default!;

    [Inject]
    private TenantService TenantService { get; set; } = default!;

    [Inject]
    private InvoiceService InvoiceService { get; set; } = default!;

    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    private MudForm _form = default!;

    private CreateInvoiceRequest _model = new()
    {
        BillingMonth = new DateTime(
            DateTime.Today.Year,
            DateTime.Today.Month,
            1),
        InvoiceDate = DateTime.Today,
        DueDate = DateTime.Today.AddDays(10),
        MonthlyRent = 0,
        PreviousReading = 0,
        CurrentReading = 0,
        ElectricityRate = 0,
        MaintenanceCharge = 0,
        WaterCharge = 0,
        LateFee = 0,
        Discount = 0,
        CreatedBy = "System User"
    };

    private List<PropertyDto> _properties = new();
    private List<UnitModel> _availableUnits = new();
    private List<TenantModel> _availableTenants = new();

    private bool IsLoading;
    private bool IsLoadingFlats;
    private bool IsLoadingTenants;
    private bool _isSaving;
    private DateTime? _billingMonth =
        new DateTime(
            DateTime.Today.Year,
            DateTime.Today.Month,
            1);

    private DateTime? _invoiceDate =
        DateTime.Today;

    private DateTime? _dueDate =
        DateTime.Today.AddDays(10);

    private decimal ElectricityUnits
    {
        get
        {
            var units =
                _model.CurrentReading -
                _model.PreviousReading;

            return units < 0 ? 0 : units;
        }
    }


    private decimal ElectricityAmount =>
        ElectricityUnits *
        _model.ElectricityRate;

    private decimal TotalPayable =>
        _model.MonthlyRent
        + ElectricityAmount
        + _model.MaintenanceCharge
        + _model.WaterCharge
        + _model.LateFee
        - _model.Discount;


    protected override async Task OnInitializedAsync()
    {
        await LoadPropertiesAsync();
    }

    private async Task LoadPropertiesAsync()
    {
        try
        {
            IsLoading = true;

            if (!_StateContainer.Property.IsLoaded)
            {
                await _StateContainer.Property.RefreshAsync();
            }
            _properties = _StateContainer.Property.Properties
                    .ToList();
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
    private async Task OnPropertyChanged(int propertyId)
    {
        _model.PropertyId = propertyId;
        _model.UnitId = 0;
        _model.TenantId = 0;
        _model.MonthlyRent = 0;
        _availableUnits.Clear();
        _availableTenants.Clear();

        if (propertyId <= 0)
        {
            return;
        }


        await LoadUnitsAsync(propertyId);
    }


    private async Task LoadUnitsAsync(int propertyId)
    {
        try
        {
            IsLoadingFlats = true;
            await _StateContainer.Unit.RefreshAsync();
            _availableUnits =
                _StateContainer.Unit.Units
                    .Where(x => x.PropertyId == propertyId)
                    .ToList();
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                ex.Message,
                Severity.Error);
        }
        finally
        {
            IsLoadingFlats = false;
        }
    }

    private async Task OnUnitChanged(int unitId)
    {
        _model.UnitId = unitId;
        _model.TenantId = 0;
        _availableTenants.Clear();
        _model.MonthlyRent = 0;

        if (unitId <= 0)
        {
            return;
        }

        var unit =
            _availableUnits.FirstOrDefault(
                x => x.UnitId == unitId);


        if (unit != null)
        {
            _model.MonthlyRent =
                unit.MonthlyRent ?? 0;
        }


        await LoadTenantsAsync(unitId);
    }


    private async Task LoadTenantsAsync(int unitId)
    {
        try
        {
            IsLoadingTenants = true;
            await _StateContainer.Tenant.RefreshAsync();

            _availableTenants =
                _StateContainer.Tenant.Tenants
                    .Where(x => x.UnitId == unitId)
                    .Where(x =>
                        string.Equals(
                            x.Status,
                            "Active",
                            StringComparison.OrdinalIgnoreCase))
                    .ToList();


          

            if (_availableTenants.Count == 1)
            {
                _model.TenantId =
                    _availableTenants[0].TenantId;
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                ex.Message,
                Severity.Error);
        }
        finally
        {
            IsLoadingTenants = false;
        }
    }



    private void CalculateElectricity(decimal value)
    {
        InvokeAsync(StateHasChanged);
    }

    private async Task CreateInvoiceAsync()
    {
        try
        {
            _isSaving = true;

            await InvokeAsync(StateHasChanged);


            if (_billingMonth.HasValue)
            {
                _model.BillingMonth =
                    new DateTime(
                        _billingMonth.Value.Year,
                        _billingMonth.Value.Month,
                        1);
            }

            if (_invoiceDate.HasValue)
            {
                _model.InvoiceDate =
                    _invoiceDate.Value;
            }

            if (_dueDate.HasValue)
            {
                _model.DueDate =
                    _dueDate.Value;
            }

            await _form.Validate();

            if (!_form.IsValid)
            {
                Snackbar.Add(
                    "Please correct the validation errors.",
                    Severity.Warning);

                return;
            }


            if (_model.CurrentReading <
                _model.PreviousReading)
            {
                Snackbar.Add(
                    "Current reading cannot be less than previous reading.",
                    Severity.Error);

                return;
            }


            if (_model.DueDate <
                _model.InvoiceDate)
            {
                Snackbar.Add(
                    "Due date cannot be earlier than invoice date.",
                    Severity.Error);

                return;
            }

            if (TotalPayable < 0)
            {
                Snackbar.Add(
                    "Total payable cannot be negative.",
                    Severity.Error);

                return;
            }
            await Task.Delay(1000);
            var response = await InvoiceService.CreateAsync(_model);
            if (!response.IsSuccess)
            {
                Snackbar.Add(
                    response.Message,
                    Severity.Error);

                return;
            }
            await _StateContainer.Invoice.RefreshAsync();
     
            Snackbar.Add(
                $"Invoice {response.Data?.InvoiceNumber} created successfully.",
                Severity.Success);
            NavigationManager.NavigateTo(
                "/invoice");
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

            await InvokeAsync(StateHasChanged);
        }
    }


    private string GetPropertyName(int propertyId)
    {
        return _properties
                   .FirstOrDefault(
                       x => x.PropertyId == propertyId)
                   ?.PropertyName
               ?? string.Empty;
    }


  
    private string GetUnitDisplayName(int unitId)
    {
        return _availableUnits
                   .FirstOrDefault(
                       x => x.UnitId == unitId)
                   ?.UnitNumber
               ?? string.Empty;
    }



    private void GoBack()
    {
        NavigationManager.NavigateTo("/invoice");
    }
}