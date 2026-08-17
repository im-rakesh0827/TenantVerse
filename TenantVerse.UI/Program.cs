using Blazored.LocalStorage;

using Microsoft.AspNetCore.Components.Authorization;

using MudBlazor.Services;

using TenantVerse.UI.Components;
using TenantVerse.UI.Services;
using TenantVerse.UI.Services.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);


// ------------------------------------------------------------
// Razor Components
// ------------------------------------------------------------
builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();

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
// builder.Services.AddAuthorizationCore();
// builder.Services
//     .AddAuthentication(options =>
//     {
//         options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//         options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//     })
//     .AddCookie();

// builder.Services.AddAuthorization();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<JwtAuthenticationStateProvider>();

builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<JwtAuthenticationStateProvider>());

// ------------------------------------------------------------
// HTTP Message Handler
// ------------------------------------------------------------
builder.Services.AddTransient<AuthorizationMessageHandler>();


// ------------------------------------------------------------
// HttpClient
// ------------------------------------------------------------
// builder.Services.AddHttpClient("TenantVerseAPI", client =>
// {
//     client.BaseAddress = new Uri("https://localhost:7148/");
// })
// .AddHttpMessageHandler<AuthorizationMessageHandler>();


builder.Services.AddHttpClient("TenantVerseAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7148/");
});


builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();

    return factory.CreateClient("TenantVerseAPI");
});


// ------------------------------------------------------------
// Application Services
// ------------------------------------------------------------
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<UnitService>();
builder.Services.AddScoped<TenantService>();
builder.Services.AddScoped<InvoiceService>();
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