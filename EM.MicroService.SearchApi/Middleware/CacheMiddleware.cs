using System.Text;
// using EM.MicroService.SearchApi.Abstractions;
using EM.MicroService.SearchApi.Helpers;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace EM.MicroService.SearchApi.Middleware;

public class CacheMiddleware
{
    private readonly IDistributedCache _distributedCache;
    private readonly RequestDelegate _next;
    private readonly static DistributedCacheEntryOptions distributedCacheEntryOptions
            = new DistributedCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(5));
    public CacheMiddleware(RequestDelegate next, IDistributedCache distributedCache)
    {
        _next = next;
        // _distributedCache = distributedCache;
        _distributedCache = distributedCache;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Path.Equals("/api/v1/Search"))
        {
            // var redisKey = RedisKeyHelper.GetRedisKey(JsonConvert.SerializeObject(context.Request.Query));
            var redisKey = JsonConvert.SerializeObject(context.Request.Query);
            // var cache = await _distributedCache.GetPreferencesFromCache(redisKey);
            var cache = await _distributedCache.GetAsync(redisKey);

            if (cache != null)
            {
                await context.Response.Body.WriteAsync(cache);
            }
            else
            {
                var responseStream = context.Response.Body;
                var ms = new MemoryStream();
                context.Response.Body = ms;

                await _next.Invoke(context);

                ms.Position = 0;
                var searchResult = ms.ToArray();
                // await _distributedCache.AddPreferencesToCache(redisKey, searchResult);
                await _distributedCache.SetAsync(redisKey, searchResult, distributedCacheEntryOptions);
                ms.Position = 0;
                await ms.CopyToAsync(responseStream);
                context.Response.Body = responseStream;
            }
        }
        else
        {
            await _next.Invoke(context);
        }
    }
}