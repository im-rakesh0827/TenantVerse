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
        protected bool IsLoading {get; set;} = false;
        protected async Task Save()
        {
            IsLoading = true;
            var propertyId = await PropertyService.CreateAsync(_model);
            if (propertyId > 0)
            {
                await Task.Delay(1000);
                Snackbar.Add("Property created successfully.", Severity.Success);
                var properties = await PropertyService.GetAllAsync();
                _StateContainer.Property.SetProperties(properties);
                Navigation.NavigateTo("/property");
            }
            else
            {
                Snackbar.Add("Failed to create property.", Severity.Error);
            }
            IsLoading = false;
        }

        protected void Cancel()
        {
            Navigation.NavigateTo("/property");
        }
    }
}