using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.UI.Services;
using TenantVerse.Shared.Helpers;

namespace TenantVerse.UI.Components.Pages.Unit;

public partial class UnitList 
{
    [Inject]
    protected UnitService UnitService { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    [Inject]
    protected StateContainer _StateContainer { get; set; } = default!;
    protected List<UnitModel> _units = new();
    protected bool IsLoading;
    protected string? _errorMessage;
    private string _searchString = string.Empty;

    private string _flatStatus="All";

    protected override async Task OnInitializedAsync()
    {
        await LoadUnitsAsync();
    }
    protected async Task LoadUnitsAsync()
    {
        _errorMessage = null;
        try
        {
            if(!_StateContainer.Unit.IsLoaded)
            {
                IsLoading = true;
                await Task.Delay(1000);
                var response = await UnitService.GetAllAsync();
                if (response == null)
                {
                    _errorMessage =
                        "Unable to load flats. No response received from the API.";
                    return;
                }
                if (!response.IsSuccess)
                {
                    _errorMessage =
                        string.IsNullOrWhiteSpace(response.Message)
                            ? "Unable to load flats."
                            : response.Message;
                    return;
                }
                _StateContainer.Unit.SetUnits(response.Data.ToList());
            }
            _units = _StateContainer.Unit.Units;
        }
        catch (HttpRequestException)
        {
            _errorMessage =
                "Unable to connect to TenantVerse API.";
        }
        catch (Exception ex)
        {
            _errorMessage =
                $"Unable to load flats: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }


    protected void AddUnit()
    {
        Navigation.NavigateTo("/flat/add");
    }

    protected void EditOrViewUnit(int unitId, string mode)
    {
        try
        {
            var _SelectedUnit = _StateContainer.Unit.Units.FirstOrDefault(x=>x.UnitId==unitId);
            _StateContainer.Unit.SetSelectedUnit(_SelectedUnit);
            _StateContainer.Unit.SetPropertyId(_SelectedUnit.PropertyId);
            Navigation.NavigateTo($"/flat/{mode}/{unitId}");
        }
        catch (System.Exception)
        {
            throw;
        }
    }

    private async Task DeleteUnit(int unitId)
    {
        try
        {
            IsLoading = true;
            var response = await UnitService.DeleteAsync(unitId);
            if (!response.IsSuccess)
            {
                Snackbar.Add(
                    response.Message,
                    Severity.Error);

                return;
            }
            Snackbar.Add("Flat deactivated successfully.",Severity.Success);
            // _StateContainer.Unit.Units.RemoveAll(x => x.UnitId == unitId);
            _StateContainer.Unit.ResetLoaded();
            await LoadUnitsAsync();
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


    protected bool IsAvailable(string? status)
    {
        return string.Equals(
            status,
            "Available",
            StringComparison.OrdinalIgnoreCase);
    }


    protected bool IsOccupied(string? status)
    {
        return string.Equals(
            status,
            "Occupied",
            StringComparison.OrdinalIgnoreCase);
    }

    // private bool FilterUnit(UnitModel unit)
    // {
    //     if (string.IsNullOrWhiteSpace(_searchString))
    //         return true;

    //     return
    //         (unit.UnitNumber?.Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.PropertyName?.Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.UnitType?.Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.FloorNumber?.ToString().Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.Bedrooms?.ToString().Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.Bathrooms?.ToString().Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.Area?.ToString().Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.MonthlyRent?.ToString().Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.SecurityDeposit?.ToString().Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false)

    //         ||

    //         (unit.Status?.Contains(
    //             _searchString,
    //             StringComparison.OrdinalIgnoreCase) ?? false);
    // }


private bool FilterUnit(UnitModel unit)
{
    // ==========================================
    // SEARCH FILTER
    // ==========================================

    if (!string.IsNullOrWhiteSpace(_searchString))
    {
        var search = _searchString.Trim();

        var matchesSearch =
            (unit.UnitNumber?.Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.PropertyName?.Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.UnitType?.Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.FloorNumber?.ToString().Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.Bedrooms?.ToString().Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.Bathrooms?.ToString().Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.Area?.ToString().Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.MonthlyRent?.ToString().Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.SecurityDeposit?.ToString().Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false)

            ||

            (unit.Status?.Contains(
                search,
                StringComparison.OrdinalIgnoreCase) ?? false);

        if (!matchesSearch)
        {
            return false;
        }
    }


    // ==========================================
    // STATUS FILTER
    // ==========================================

    if (!string.IsNullOrWhiteSpace(_flatStatus) &&
        !string.Equals(
            _flatStatus,
            "All",
            StringComparison.OrdinalIgnoreCase))
    {
        if (!string.Equals(
                unit.Status?.Trim(),
                _flatStatus.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
    }


    // ==========================================
    // MATCHED ALL FILTERS
    // ==========================================

    return true;
}


    protected IEnumerable<UnitModel> FilteredUnits =>
        string.IsNullOrWhiteSpace(_searchString)
        && string.IsNullOrWhiteSpace(_flatStatus)
            ? _units
            : _units.Where(FilterUnit);
    
}