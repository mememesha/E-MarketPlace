using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using EM.UI.Blazor;
using EM.UI.Blazor.Services;
using EM.UI.Blazor.Settings;
using EM.UI.Blazor.Handlers;
using Blazored.SessionStorage;
using Blazored.Modal;
using Blazored.LocalStorage;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Подключение отдельного HttpClient для отправки JWT
builder.Services.AddScoped<CustomAuthorizationMessageHandler>();
builder.Services.AddHttpClient("WebAPI",
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
    .AddHttpMessageHandler<CustomAuthorizationMessageHandler>();

// HttpClient для всех запросов, не требующих авторизацию
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("IS4", options.ProviderOptions);
});

builder.Services.Configure<WebApiOptions>(options =>
    builder.Configuration.Bind("WebApiOptions", options));

builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredModal();
builder.Services.AddBlazoredLocalStorage(config =>
{
    config.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    config.JsonSerializerOptions.IgnoreReadOnlyProperties = true;
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    config.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    config.JsonSerializerOptions.ReadCommentHandling = JsonCommentHandling.Skip;
    config.JsonSerializerOptions.WriteIndented = false;
});

builder.Services.AddScoped<SearchService>();
builder.Services.AddScoped<BasketService>();
builder.Services.AddSingleton<UserService>();

builder.Services.AddOptions();
builder.Services.AddAuthorizationCore();
// builder.Services.AddApiAuthorization();

await builder.Build().RunAsync();