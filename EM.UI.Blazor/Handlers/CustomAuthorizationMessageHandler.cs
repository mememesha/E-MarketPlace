using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace EM.UI.Blazor.Handlers;

public class CustomAuthorizationMessageHandler : AuthorizationMessageHandler
{
    //TODO Задавать настройки через DI
    public CustomAuthorizationMessageHandler(IAccessTokenProvider provider,
        NavigationManager navigationManager)
        : base(provider, navigationManager)
    {
        ConfigureHandler(
            authorizedUrls: new[] { "https://localhost:7890" },
            scopes: new[] { "webapi.write" });
    }
}