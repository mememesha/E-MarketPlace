using System.Collections.Specialized;
using System.Web;
using EM.UI.Blazor.Settings;
using Microsoft.Extensions.Options;
using EM.Dto;
using System.Net.Http.Json;

namespace EM.UI.Blazor.Services;

public class SearchService
{
    private readonly IOptions<WebApiOptions> _options;
    private readonly HttpClient _client;
    private string? SearchPath => _options.Value.Ip + _options.Value.SearchPath;
    public string? Query { get; set; }
    public string? Category { get; set; }
    public string? Location { get; set; }
    public event Action? OnSearch;

    public SearchService(IOptions<WebApiOptions> options, HttpClient client)
    {
        _options = options;
        _client = client;
    }

    public void OnSearchExecute() => OnSearch?.Invoke();

    public async Task<List<OfferShortResponseDto>?> GetSearchResultAsync()
    {
        var response = await _client.GetAsync(SearchPath! + GetQuery());

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<List<OfferShortResponseDto>>();
    }

    public NameValueCollection GetQuery()
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        query["query"] = Query;

        if (!string.IsNullOrEmpty(Category))
            query["category"] = Category;

        if (!string.IsNullOrEmpty(Location))
            query["location"] = Location;

        return query;
    }
}