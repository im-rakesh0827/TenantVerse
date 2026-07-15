using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace TenantVerse.UI.Components.Pages.Property
{
    public partial class UpdateOrViewProperty
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
          protected PropertyState PropertyState { get; set; } = default!;
           [Parameter]
           public int Id { get; set; }
        //   [SupplyParameterFromQuery]
        public bool IsEditMode { get; set; }
        public string Title{get; set;} = "Edit Property";
        public bool IsDisabled {get; set;} = false;

        protected bool IsLoading{get; set;} = false;
        PropertyDto propertyModel { get; set; } = new PropertyDto();
        protected override async Task OnInitializedAsync(){
            try{
                IsLoading = true;
                if (PropertyState.SelectedProperty == null && string.IsNullOrWhiteSpace(PropertyState.SelectedProperty.Email))
                {
                    propertyModel = await PropertyService.GetByIdAsync(Id);
                }                
                else{
                    propertyModel = PropertyState.SelectedProperty;
                }
                IsEditMode = PropertyState._IsEditMode;
                IsDisabled = !IsEditMode;
                Title = IsEditMode ? "Edit Property" : "View Property";
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
            }
            finally{
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }
        }

        public void Cancel()
        {
            Navigation.NavigateTo("/property");
        }

        public async Task UpdateProperty(){
            
            try{
                IsLoading = true;
                var result = await PropertyService.UpdateAsync(propertyModel);
                if(result){
                    PropertyState.Clear();
                    var data = await PropertyService.GetAllAsync();
                    PropertyState.Set(data);
                    await Task.Delay(1000);
                    Snackbar.Add("Property updated successfully.", Severity.Success);
                    Navigation.NavigateTo("/property");
                }
                else{
                    Snackbar.Add("Failed to update property.", Severity.Error);
                }
            }
            catch(Exception ex){
                Console.WriteLine(ex.Message);
                Snackbar.Add("An error occurred while updating the property.", Severity.Error);
            }
            finally{
                IsLoading = false;
                await InvokeAsync(StateHasChanged);
            }

        }

        
    }
}