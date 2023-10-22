using System.Web;
using EM.UI.Blazor.Settings;
using Microsoft.Extensions.Options;

namespace EM.UI.Blazor.Services;

// Перенести логику адресов на сторону API
public class LocationService
{
    private readonly HttpClient _client;
    private readonly IOptions<WebApiOptions> _options;

    private string? GetCityFromIpRequestUrl => _options.Value.Ip + _options.Value.GetCityFromIp;

    public LocationService(IOptions<WebApiOptions> options, HttpClient client)
    {
        _client = client;
        _options = options;
    }
    
    public async Task<string> GetCityAsync()
        => await _client.GetStringAsync(GetCityFromIpRequestUrl);
}