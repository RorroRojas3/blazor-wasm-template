using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using RR.UI;
using RR.UI.Auth;
using RR.UI.Services;
using RR.UI.Stores;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]!;
var apiScopes = builder.Configuration.GetSection("ApiSettings:Scopes").Get<string[]>() ?? [];

// MSAL authentication — automatic redirect, no login button
builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAd", options.ProviderOptions.Authentication);
    options.ProviderOptions.LoginMode = "redirect";

    foreach (var scope in apiScopes)
    {
        options.ProviderOptions.DefaultAccessTokenScopes.Add(scope);
    }
});

// Named HttpClient for the backend API — Bearer token attached automatically
builder.Services.AddHttpClient("RR.Api",
    client => client.BaseAddress = new Uri(apiBaseUrl))
    .AddHttpMessageHandler(sp =>
        sp.GetRequiredService<AuthorizationMessageHandler>()
            .ConfigureHandler(
                authorizedUrls: [apiBaseUrl],
                scopes: apiScopes));

builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<ReferenceStore>();
builder.Services.AddScoped<VersionCacheService>();

// Plain HttpClient for local static files (e.g. sample-data)
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

var host = builder.Build();

// Run once at startup: clear stale localStorage when a new version is deployed
await using var versionCache = host.Services.GetRequiredService<VersionCacheService>();
await versionCache.CheckAndBustCacheAsync();

await host.RunAsync();
