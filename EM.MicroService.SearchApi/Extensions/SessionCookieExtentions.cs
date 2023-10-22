using EM.MicroService.SearchApi.Middleware;

namespace EM.MicroService.SearchApi.Extensions;

public static class SessionCookieExtensions
{
    public static IApplicationBuilder UseSessionCookie(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionCookieMiddleware>();
    }
}