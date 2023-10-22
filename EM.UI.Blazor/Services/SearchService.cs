using EM.UI.Blazor.Settings;
using Microsoft.Extensions.Options;

namespace EM.UI.Blazor.Services;

public class SearchService
{
    private readonly IOptions<WebApiOptions> _options;

    public string? Query { get; set; }

    public string? Filter { get; set; }

    public string? SearchPath => _options.Value.Ip + _options.Value.SearchPath;

    public event Action? OnSearch;

    public SearchService(IOptions<WebApiOptions> options)
    {
        _options = options;
    }
    
    public void SearchExecute() => OnSearch?.Invoke();
}