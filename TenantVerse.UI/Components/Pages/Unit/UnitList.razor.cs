using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.UI.Services;

namespace TenantVerse.UI.Components.Pages.Unit;

public partial class UnitList 
{
    [Inject]
    protected UnitService UnitService { get; set; } = default!;
    [Inject]
    protected ISnackbar Snackbar { get; set; } = default!;
    [Inject]
    protected NavigationManager Navigation { get; set; } = default!;
    protected List<UnitModel> _units = new();
    protected bool _isLoading;
    protected string? _errorMessage;
    protected override async Task OnInitializedAsync()
    {
        await LoadUnitsAsync();
    }
    protected async Task LoadUnitsAsync()
    {
        _isLoading = true;
        _errorMessage = null;

        try
        {
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

            _units = response.Data ?? new List<UnitModel>();
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
            _isLoading = false;
        }
    }


    protected void AddUnit()
    {
        Navigation.NavigateTo("/flat/add");
    }


    protected void ViewUnit(int unitId)
    {
        Navigation.NavigateTo($"/flat/view/{unitId}");
    }


    protected void EditUnit(int unitId)
    {
        Navigation.NavigateTo($"/flat/edit/{unitId}");
    }

    private async Task DeleteUnit(int unitId)
    {
        try
        {
            _isLoading = true;
    
            var response = await UnitService.DeleteAsync(unitId);
    
            if (!response.IsSuccess)
            {
                Snackbar.Add(
                    response.Message,
                    Severity.Error);
    
                return;
            }
    
            Snackbar.Add(
                "Flat deactivated successfully.",
                Severity.Success);
    
            await LoadUnitsAsync();
        }
        catch (Exception ex)
        {
            Snackbar.Add(
                ex.Message,
                Severity.Error);
        }
        finally
        {
            _isLoading = false;
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
}