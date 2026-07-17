using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using RetailSphere.UI;
using RetailSphere.UI.Clients;
using RetailSphere.UI.Configuration.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? builder.HostEnvironment.BaseAddress;

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();

// Scoped, not Singleton — it depends on Blazored.LocalStorage's ILocalStorageService,
// which is registered Scoped. A WASM app only ever has one scope for its whole
// lifetime, so this behaves like a singleton in practice while still satisfying
// the DI container's lifetime validation.
builder.Services.AddScoped<JwtAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<JwtAuthenticationStateProvider>());

builder.Services.AddTransient<JwtTokenMessageHandler>();

// Anonymous client — used only by JwtTokenMessageHandler itself to call /auth/refresh
// without recursing back through the auth handler.
builder.Services.AddHttpClient("RetailSphereApi.Anonymous", client => client.BaseAddress = new Uri(apiBaseUrl));

builder.Services.AddHttpClient("RetailSphereApi", client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler<JwtTokenMessageHandler>();

builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("RetailSphereApi"));
builder.Services.AddScoped<IApiClient, ApiClient>();

await builder.Build().RunAsync();
