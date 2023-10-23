using System.Net.Http.Json;

namespace EM.UI.Blazor.Handlers;

public static class DataHandler
{
    public static async Task<T?> LoadAsync<T>(string requestUrl, HttpClient client)
        where T : new()
    {
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            return default;

        var result = await response.Content.ReadFromJsonAsync<T>();

        return result;
    }

    public static async Task<string?> LoadAsync(string requestUrl, HttpClient client)
    {
        var response = await client.GetAsync(requestUrl);

        if (!response.IsSuccessStatusCode)
            return default;

        var result = await response.Content.ReadAsStringAsync();

        return result;
    }

    public static async Task<bool> SaveAsync<T>(T obj, string requestUrl, HttpClient client)
        where T : new()
    {
        var response = await client.PutAsJsonAsync(requestUrl, obj);

        if (!response.IsSuccessStatusCode)
            return false;

        return true;
    }

    public static async Task<bool> AddAsync<T>(T obj, string requestUrl, HttpClient client)
        where T : new()
    {
        var response = await client.PostAsJsonAsync(requestUrl, obj);

        return response.IsSuccessStatusCode;
    }
}