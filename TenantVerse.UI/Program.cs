using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using MudBlazor.Services;

using TenantVerse.UI.Components;
using TenantVerse.UI.Services;
using TenantVerse.UI.Services.Authentication;

var builder = WebApplication.CreateBuilder(args);


// ------------------------------------------------------------
// Razor Components
// ------------------------------------------------------------
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();


// ------------------------------------------------------------
// MudBlazor
// ------------------------------------------------------------
builder.Services.AddMudServices();


// ------------------------------------------------------------
// Local Storage
// ------------------------------------------------------------
builder.Services.AddBlazoredLocalStorage();


// ------------------------------------------------------------
// Authentication / Authorization
// ------------------------------------------------------------
builder.Services.AddAuthorizationCore();

// builder.Services.AddScoped<JwtAuthenticationStateProvider>();

// builder.Services.AddScoped<AuthenticationStateProvider>(
//     provider => provider.GetRequiredService<JwtAuthenticationStateProvider>());


// ------------------------------------------------------------
// HTTP Message Handler
// ------------------------------------------------------------
builder.Services.AddTransient<AuthorizationMessageHandler>();


// ------------------------------------------------------------
// HttpClient
// ------------------------------------------------------------
builder.Services.AddHttpClient("TenantVerseAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7148/");
})
.AddHttpMessageHandler<AuthorizationMessageHandler>();


builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();

    return factory.CreateClient("TenantVerseAPI");
});


// ------------------------------------------------------------
// Application Services
// ------------------------------------------------------------
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<PropertyService>();

builder.Services.AddScoped<StateContainer>();


var app = builder.Build();


// ------------------------------------------------------------
// Configure HTTP Pipeline
// ------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();


// ------------------------------------------------------------
// Razor Components
// ------------------------------------------------------------
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();


app.Run();