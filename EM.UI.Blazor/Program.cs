using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EM.UI.Blazor;
using EM.UI.Blazor.Services;
using EM.Dto.Settings;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)});

builder.Services.AddOidcAuthentication(options =>
{
    // Configure your authentication provider options here.
    // For more information, see https://aka.ms/blazor-standalone-auth
    builder.Configuration.Bind("IS4", options.ProviderOptions);
});
builder.Services.Configure<WebApiOptions>(options =>
    builder.Configuration.Bind("WebApiOptions", options));
builder.Services.AddSingleton<SearchService>();

await builder.Build().RunAsync();