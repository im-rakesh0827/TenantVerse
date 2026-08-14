using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TenantVerse.UI.Components.Pages.Property
{
    public partial class PropertyList
    {
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
          protected bool IsLoading = true;
          protected int TotalPropertyCount {get; set;} = 0;
          private string? _errorMessage;
          protected string SearchString = string.Empty;
          protected IEnumerable<PropertyDto> FilteredProperties =>string.IsNullOrWhiteSpace(SearchString)? properties:properties.Where(FilterProperty);

          protected override async Task OnInitializedAsync()
          {
               try
               {
                    _errorMessage = null;
                    StateHasChanged();
                    await LoadProperties();
               }
               catch (Exception ex)
               {
                    _errorMessage = ex.Message;
                    throw;
               }
          }
          private void NavigateToCreate()
          {
               Navigation.NavigateTo("/property/create");
          }

          private async Task DeleteProperty(int propertyId){
               string name = properties.FirstOrDefault(p => p.PropertyId == propertyId)?.PropertyName ?? "this property";
               string userName = "TestUser";
               var parameters = new DialogParameters
               {
                    { nameof(ConfirmDialog.Title), "Delete Confirmation" },
                    { nameof(ConfirmDialog.Message), $"Are you sure you want to delete '{name}'?" },
                    { nameof(ConfirmDialog.ConfirmButtonText), "Delete" }
               };

               var options = new DialogOptions
               {
                    CloseOnEscapeKey = true,
                    MaxWidth = MaxWidth.ExtraSmall,
                    FullWidth = true
               };

               var dialog = await DialogService.ShowAsync<ConfirmDialog>("", parameters, options);
               var result = await dialog.Result;

               if (result is not null && !result.Canceled)
               {
                    IsLoading = true;
                    await InvokeAsync(StateHasChanged);
                    await Task.Delay(1000);
                    var isDeleted = await PropertyService.DeleteAsync(propertyId,userName);
                    if (isDeleted)
                    {                        
                         // _StateContainer.Property.Properties.RemoveAll(x => x.PropertyId == propertyId);
                         Snackbar.Add("Property deleted successfully.", Severity.Success);
                         _StateContainer.Property.ResetLoaded();
                         await LoadProperties();
                    }
                    else
                    {
                         Snackbar.Add("Failed to delete property.", Severity.Error);
                    }
               }
          }

          private async Task LoadProperties(){
               IsLoading = true;
               if (!_StateContainer.Property.IsLoaded)
               {
                    await Task.Delay(1000);
                    var data = await PropertyService.GetAllAsync();
                    _StateContainer.Property.SetProperties(data);
               }
               properties = _StateContainer.Property.Properties;
               TotalPropertyCount =properties.Count();
               IsLoading = false;
               Navigation.NavigateTo("/property");
          }

          private void UpdateOrView(int id, string mode)
          {   
               var property = _StateContainer.Property.Properties.FirstOrDefault(x=>x.PropertyId==id);
               _StateContainer.Property.SetSelectedProperty(property);
               _StateContainer.Property.SetPropertyId(id);
               Navigation.NavigateTo($"/property/{mode}/{id}");
          }

          private bool FilterProperty(PropertyDto property)
          {
               if (string.IsNullOrWhiteSpace(SearchString))
                    return true;

               return
                    property.PropertyCode.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    property.PropertyName.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    property.OwnerName.Contains(SearchString, StringComparison.OrdinalIgnoreCase) ||
                    property.City.Contains(SearchString, StringComparison.OrdinalIgnoreCase)||
                    property.TotalFloors.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)||
                    property.Email.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase)||
                    property.TotalFlats.ToString().Contains(SearchString, StringComparison.OrdinalIgnoreCase);
          }
     }
}
