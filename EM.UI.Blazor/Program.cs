using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EM.UI.Blazor;
using EM.UI.Blazor.Services;
using EM.UI.Blazor.Settings;
using EM.UI.Blazor.Handlers;
using System.Net.Http;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Подключение отдельного HttpClient для отправки JWT
// builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
// builder.Services.AddHttpClient("WebAPI",
//     client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
//     .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();
// builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
//     .CreateClient("WebAPI"));

builder.Services.AddScoped(sp => new HttpClient {
        BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
    });

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("IS4", options.ProviderOptions);
});

builder.Services.Configure<WebApiOptions>(options =>
    builder.Configuration.Bind("WebApiOptions", options));

builder.Services.AddScoped<LocationService>();
builder.Services.AddScoped<SearchService>();

await builder.Build().RunAsync();