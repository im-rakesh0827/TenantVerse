using MudBlazor.Services;
using TenantVerse.UI.Components;
using TenantVerse.UI.Services;
using Blazored.LocalStorage;
var builder = WebApplication.CreateBuilder(args);


// builder.Services.AddHttpClient("TenantVerseApi", client =>
// {
//     client.BaseAddress = new Uri("https://localhost:5001/");
// });

// Razor Components
// builder.Services.AddRazorComponents()
//     .AddInteractiveServerComponents();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });

// builder.Services.AddBlazoredLocalStorage();


// MudBlazor
builder.Services.AddMudServices();

builder.Services.AddScoped<PropertyService>();
builder.Services.AddScoped<PropertyState>();

// HttpClient (API)
builder.Services.AddHttpClient("TenantVerseAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7148/");
});

// Default HttpClient
builder.Services.AddScoped(sp =>
{
    var factory = sp.GetRequiredService<IHttpClientFactory>();
    return factory.CreateClient("TenantVerseAPI");
});

builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();