using System.Net;
using System.Text;
using EM.MicroService.SearchApi.Abstractions;
using EM.MicroService.SearchApi.Models;
using Newtonsoft.Json;

namespace EM.MicroService.SearchApi;

public class DistributedCache : IDistributedCache
{
    private readonly HttpClient _httpClient;

    public DistributedCache(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> AddPreferencesToCache(Guid redisKey, byte[] preferenceResponses)
    {
        var response = await _httpClient.PostAsync($"api/v1/Search?redisKey={redisKey}", 
                new StringContent(JsonConvert.SerializeObject(preferenceResponses), Encoding.UTF8, "application/json"));

        return response.StatusCode == HttpStatusCode.OK && JsonConvert.DeserializeObject<bool>(await response.Content.ReadAsStringAsync());
    }

    public async Task<byte[]> GetPreferencesFromCache(Guid redisKey)
    {
            var response = await _httpClient.GetAsync($"api/v1/Search?redisKey={redisKey}");
            return response.StatusCode == HttpStatusCode.OK 
                ? await response.Content.ReadAsByteArrayAsync()
                : null;

    }
}
