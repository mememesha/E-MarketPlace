using EM.Dto;
using EM.UI.Blazor.Handlers;
using EM.UI.Blazor.Settings;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Options;

namespace EM.UI.Blazor.Services;

public class UserService : IDisposable
{
    private bool IsAuthenticated { get; set; }
    public UserDto? User { get; private set; }
    public List<OfferShortResponseDto>? Offers { get; private set; }
    public List<ReserveDto>? Reserves { get; private set; }

    private event Action? OnUserDataGet;

    public async Task<bool> IsLoaded()
    {
        TaskCompletionSource<bool> tcs = new();

        OnUserDataGet += () =>
        {
            tcs.SetResult(true);
        };

        return IsAuthenticated || await tcs.Task;
    }

    public void SetUserData(UserDto? user, List<OfferShortResponseDto> offers, List<ReserveDto>? reserves)
    {
        if (user != null)
        {
            User = user;
            Offers = offers;
            Reserves = reserves ?? new List<ReserveDto>();
            IsAuthenticated = true;
        }

        OnUserDataGet?.Invoke();
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);
    }
}