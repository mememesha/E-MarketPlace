namespace EM.MicroService.SearchApi.Middleware;

public class SessionCookieMiddleware
{
    private readonly RequestDelegate next;

    public SessionCookieMiddleware(RequestDelegate next)
    {
        this.next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if(!context.Request.Cookies.ContainsKey("session_id"))
        {
            var cookieOptions = new CookieOptions
            {
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None,
                IsEssential = true
            };

            context.Response.Cookies.Append("session_id", Guid.NewGuid().ToString());
            
        }
                
        await next.Invoke(context);
    }
}