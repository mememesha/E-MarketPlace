using EM.WebApi.Core.Middleware;

namespace EM.WebApi.Core.Extensions;

public static class SessionCookieExtensions
{
    public static IApplicationBuilder UseSessionCookie(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SessionCookieMiddleware>();
    }
}