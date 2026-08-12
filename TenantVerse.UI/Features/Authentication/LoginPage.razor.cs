

using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Authentication.Requests;
using TenantVerse.UI.Services.Authentication;
using Blazored.LocalStorage;

namespace TenantVerse.UI.Features.Authentication;

public partial class LoginPage
{
    private MudForm? _form;
     [Inject]
    private AuthService AuthService { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;
    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    private bool IsLoading;

    private LoginRequest _model = new();


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        try
        {
            if (!await AuthService.IsLoggedInAsync())
            {
                NavigationManager.NavigateTo("/login");
            }
            else
            {
                NavigationManager.NavigateTo("/dashboard");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);

            NavigationManager.NavigateTo("/login");
        }
    }

    private async Task LoginAsync()
    {
        await _form!.Validate();

        if (!_form.IsValid)
            return;

        IsLoading = true;

        try
        {
            var response = await AuthService.LoginAsync(_model);
            if (response == null)
            {
                Snackbar.Add("Unable to connect to server.", Severity.Error);
                return;
            }

            if (response.IsSuccess)
            {
                Snackbar.Add(response.Message, Severity.Success);
                _model = new LoginRequest();
                await Task.Delay(1000);
                NavigationManager.NavigateTo("/dashboard");
            }
            else
            {
                Snackbar.Add(response.Message, Severity.Error);
            }

        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }
}