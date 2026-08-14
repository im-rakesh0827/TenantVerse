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
        protected StateContainer _StateContainer{get; set;} = default;
        [Parameter]
        public int Id { get; set; }
        [Parameter]
        public string Mode { get; set; } = string.Empty;
        // [SupplyParameterFromQuery]
        public bool IsEditMode { get; set; }
        public string Title{get; set;} = "Edit Property";
        public bool IsDisabled {get; set;} = false;
        protected bool IsLoading{get; set;} = false;
        PropertyDto propertyModel { get; set; } = new PropertyDto();
        protected override async Task OnInitializedAsync()
        {
            IsDisabled = !(Mode == "edit");
            Title = Mode== "edit" ? "Edit Property" : "View Property";
            await LoadProperty();
        }

        private async Task LoadProperty()
        {
            IsLoading = true;
            try
            {
                // var property = _StateContainer.Property.Properties.FirstOrDefault(x => x.PropertyId == Id);
                var property = _StateContainer.Property.SelectedProperty;
                if (property is not null)
                {
                    propertyModel = property;
                }
                else
                {
                    property = await PropertyService.GetByIdAsync(Id);
                    propertyModel = property;
                    // Console.WriteLine($"Selected Property Name : {_StateContainer.Property.SelectedProperty.PropertyId}");
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void Cancel()
        {
            Navigation.NavigateTo("/property");
        }

        public async Task UpdateProperty()
        {
            try
            {
                IsLoading = true;
                var result = await PropertyService.UpdateAsync(propertyModel);
                if(result){
                    _StateContainer.Property.Clear();
                    var data = await PropertyService.GetAllAsync();
                    _StateContainer.Property.SetProperties(data);
                    // _StateContainer.Property.ResetLoaded();
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