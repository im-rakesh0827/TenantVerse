using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Tenant;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Property;
using TenantVerse.UI.Services;

namespace TenantVerse.UI.Components.Pages.Tenant;

public partial class UpdateOrViewTenant
{
    // ============================================================
    // PARAMETER
    // ============================================================

    [Parameter]
    public int TenantId { get; set; }
    [Parameter]
    public string Mode {get;set;} = string.Empty;


    // ============================================================
    // SERVICES
    // ============================================================

    [Inject]
    private TenantService TenantService { get; set; } = default!;

    [Inject]
    private PropertyService PropertyService { get; set; } = default!;

    [Inject]
    private UnitService UnitService { get; set; } = default!;

    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;

    // ============================================================
    // FORM
    // ============================================================

    private MudForm? _form;


    // ============================================================
    // MODEL
    // ============================================================

    private UpdateTenantRequest _model = new();


    // ============================================================
    // DATA
    // ============================================================

    private List<PropertyDto> _properties = [];

    private List<UnitModel> _units = new();


    // ============================================================
    // TENANT STATUS
    // ============================================================

    private readonly List<string> _tenantStatuses =
    [
        "Active",
        "Inactive",
        "Pending"
    ];


    // ============================================================
    // STATE
    // ============================================================

    private bool _isPageLoading;
    private bool _isLoadingFlats;
    private bool _isSaving;
    private string? _message;
    private Severity _severity = Severity.Info;
    private bool IsLoading{get; set;} = false;
     // Severity.Info
     // Severity.Success
     // Severity.Warning
     // Severity.Error

    // ============================================================
    // VIEW / EDIT MODE
    // ============================================================

//     private bool IsReadOnly =>NavigationManager.Uri.Contains("/tenant/view/",StringComparison.OrdinalIgnoreCase);
    private bool IsReadOnly{get; set;} = false;


    // ============================================================
    // INITIALIZATION
    // ============================================================

    protected override async Task OnInitializedAsync()
    {
        IsReadOnly = !(Mode == "edit");
        await LoadPropertiesAsync();
        await LoadTenantAsync();
    }


    // ============================================================
    // LOAD PROPERTIES
    // ============================================================

     private async Task LoadPropertiesAsync(){
          IsLoading = true;
          if (!_StateContainer.Property.IsLoaded)
          {
               await Task.Delay(1000);
               var data = await PropertyService.GetAllAsync();
               _StateContainer.Property.SetProperties(data);
          }
          _properties = _StateContainer.Property.Properties;
          IsLoading = false;
     }


    // ============================================================
    // LOAD TENANT
    // ============================================================

    private async Task LoadTenantAsync()
    {
        try
        {
            _isPageLoading = true;
            _message = null;

            if (TenantId <= 0)
            {
                _message = "Invalid tenant.";

                return;
            }

            var response =
                await TenantService.GetByIdAsync(TenantId);

            if (!response.IsSuccess ||
                response.Data is null)
            {
                _message =
                    response.Message ??
                    "Unable to load tenant.";
               _severity = Severity.Info;

                return;
            }

            var tenant = response.Data;

            _model = new UpdateTenantRequest
            {
                TenantId = tenant.TenantId,

                PropertyId = tenant.PropertyId,

                UnitId = tenant.UnitId,

                FirstName = tenant.FirstName,

                LastName = tenant.LastName,

                Email = tenant.Email,

                PhoneNumber = tenant.PhoneNumber,

                EmergencyContactName =
                    tenant.EmergencyContactName,

                EmergencyContactPhone =
                    tenant.EmergencyContactPhone,

                LeaseStartDate =
                    tenant.LeaseStartDate,

                LeaseEndDate =
                    tenant.LeaseEndDate,

                MonthlyRent =
                    tenant.MonthlyRent,

                SecurityDeposit =
                    tenant.SecurityDeposit,

                Status = tenant.Status
            };


            // ----------------------------------------------------
            // Load flats belonging to current property
            // ----------------------------------------------------

            if (_model.PropertyId > 0)
            {
                await LoadUnitsAsync(
                    _model.PropertyId,
                    _model.UnitId);
            }
        }
        catch (Exception ex)
        {
            _message = ex.Message;
            _severity = Severity.Error;
        }
        finally
        {
            _isPageLoading = false;
        }
    }


    // ============================================================
    // PROPERTY CHANGED
    // ============================================================

    private async Task OnPropertyChanged(int propertyId)
    {
        _model.PropertyId = propertyId;

        _model.UnitId = 0;
        _units.Clear();

        if (propertyId <= 0)
        {
            return;
        }
        await LoadUnitsAsync(propertyId, 0);
    }

    // ============================================================
    // LOAD FLATS
    // ============================================================

    private async Task LoadUnitsAsync(
        int propertyId,
        int selectedUnitId)
    {
        try
        {
            _isLoadingFlats = true;
            _message = null;

            var response =
                await UnitService.GetByPropertyIdAsync(
                    propertyId);

            if (response.IsSuccess &&
                response.Data is not null)
            {
                _units = response.Data
                    .Where(x =>
                        x.IsActive &&
                        (
                            string.Equals(
                                x.Status,
                                "Available",
                                StringComparison.OrdinalIgnoreCase)
                            ||
                            x.UnitId == selectedUnitId
                        ))
                    .ToList();
            }
            else
            {
                _message =
                    response.Message ??
                    "Unable to load flats.";
               _severity = Severity.Info;
            }
        }
        catch (Exception ex)
        {
            _message = ex.Message;
            _severity = Severity.Error;
        }
        finally
        {
            _isLoadingFlats = false;
        }
    }


    // ============================================================
    // FLAT CHANGED
    // ============================================================

    private Task OnUnitChanged(int unitId)
    {
        _model.UnitId = unitId;

        var selectedUnit =
            _units.FirstOrDefault(
                x => x.UnitId == unitId);

        if (selectedUnit is not null)
        {
            _model.MonthlyRent =
                selectedUnit.MonthlyRent;
        }
        return Task.CompletedTask;
    }


    // ============================================================
    // SAVE / UPDATE TENANT
    // ============================================================

    private async Task SaveAsync()
    {
        if (_isSaving)
        {
            return;
        }

        try
        {
            _isSaving = true;
            IsLoading = true;

            _message = null;


            // ----------------------------------------------------
            // Validate form
            // ----------------------------------------------------

            if (_form is not null)
            {
                await _form.Validate();

                if (!_form.IsValid)
                {
                    return;
                }
            }


            // ----------------------------------------------------
            // Validate Property
            // ----------------------------------------------------

            if (_model.PropertyId <= 0)
            {
                _message = "Please select a property.";

                return;
            }


            // ----------------------------------------------------
            // Validate Flat
            // ----------------------------------------------------

            if (_model.UnitId <= 0)
            {
                _message =
                    "Please select a flat.";

                return;
            }


            // ----------------------------------------------------
            // Update Tenant
            // ----------------------------------------------------
            await Task.Delay(1000);
            var response = await TenantService.UpdateAsync(_model);
            if (response.IsSuccess)
            {
               _message = "Tenant details updated successfully.";
               _severity = Severity.Success;
               await _StateContainer.Tenant.RefreshAsync();
               NavigationManager.NavigateTo("/tenant");
            }
            else
            {
                _message =
                    response.Message ??
                    "Unable to update tenant.";
                    _severity = Severity.Info;

                    
            }
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            _isSaving = false;
            IsLoading = false;
            ShowMessage();
        }
    }


    // ============================================================
    // BACK
    // ============================================================

    private void GoBack()
    {
        NavigationManager.NavigateTo(
            "/tenant");
    }

    private void ShowMessage()
    {
          if(!string.IsNullOrWhiteSpace(_message))
          {
               Snackbar.Add(_message, _severity);
          }
      
    }









    private Task<IEnumerable<int>> SearchProperties(
    string? value,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return Task.FromResult(
            _properties.Select(x => x.PropertyId));
    }

    var result = _properties
        .Where(x =>
            x.PropertyName.Contains(
                value,
                StringComparison.OrdinalIgnoreCase))
        .Select(x => x.PropertyId);

    return Task.FromResult(result);
}

private string GetPropertyName(int propertyId)
{
    return _properties
        .FirstOrDefault(x => x.PropertyId == propertyId)
        ?.PropertyName ?? string.Empty;
}



private Task<IEnumerable<int>> SearchUnits(
    string? value,
    CancellationToken cancellationToken)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        return Task.FromResult(
            _units.Select(x => x.UnitId));
    }

    var result = _units
        .Where(x =>
            x.UnitNumber.Contains(
                value,
                StringComparison.OrdinalIgnoreCase))
        .Select(x => x.UnitId);

    return Task.FromResult(result);
}

private string GetUnitDisplayName(int unitId)
{
    return _units
        .FirstOrDefault(x => x.UnitId == unitId)
        ?.UnitNumber ?? string.Empty;
}
}