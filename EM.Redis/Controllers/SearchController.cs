using EM.Redis.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace EM.Redis.Controllers;

/// <summary>
/// Кэш 
/// </summary>
[ApiController]
[Route("api/v1/[controller]")]
public class SearchController : Controller
{
    private readonly IDatabase _redis;

    public SearchController(IDatabase redis)
    {
        _redis = redis;
    }

    /// <summary>
    /// Закэшировать.
    /// </summary>
    /// <param name="redisKey">Ключ кэша.</param>
    /// <param name="preferenceResponses">Модель для кэширование.</param>
    /// <returns>bool</returns>
    [HttpPost]
    public async Task<bool> AddSearchResponseToCache(Guid redisKey, byte[] searchResponse)
    {
        if (searchResponse != null)
        {
            return await _redis.StringSetAsync(
                redisKey.ToString(),
                searchResponse,
                flags: CommandFlags.FireAndForget,
                expiry: TimeSpan.FromMinutes(5));
        }

        return false;
    }

    /// <summary>
    /// Получить с кэша.
    /// </summary>
    /// <param name="redisKey">Ключ кэша.</param>
    /// <returns></returns>
    [HttpGet]
    public async Task<string>GetPreferencesFromCache(Guid redisKey)
    {
        string cache = await _redis.StringGetAsync(redisKey.ToString());

        return !string.IsNullOrEmpty(cache) 
            ? cache
            : null;
    }
}