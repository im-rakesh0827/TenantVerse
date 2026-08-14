using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.UI.Models.Property;
using TenantVerse.UI.Services;
using TenantVerse.UI.Components.Shared;

namespace TenantVerse.UI.Components.Pages.Property
{
    public partial class CreateProperty
    {
        [Inject]
        protected PropertyService PropertyService { get; set; } = default!;
        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;
        [Inject]
        protected ISnackbar Snackbar { get; set; } = default!;
        [Inject]
        protected StateContainer _StateContainer {get; set;} = default;
        protected CreatePropertyRequest _model = new();

        protected async Task Save()
        {
            var propertyId = await PropertyService.CreateAsync(_model);

            if (propertyId > 0)
            {
                Snackbar.Add("Property created successfully.", Severity.Success);
                _StateContainer.Property.ResetLoaded();
                Navigation.NavigateTo("/property");
            }
            else
            {
                Snackbar.Add("Failed to create property.", Severity.Error);
            }
        }

        protected void Cancel()
        {
            Navigation.NavigateTo("/property");
        }
    }
}