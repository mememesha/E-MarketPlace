using EM.MicroService.SearchApi.Middleware;

namespace EM.MicroService.SearchApi.Extensions;

public static class CacheExtension
{
    public static IApplicationBuilder UseCache(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CacheMiddleware>();
    }
}