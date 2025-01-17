using Microsoft.Extensions.Caching.Memory;

namespace MySuperDuperPetProject.Middlewares
{
    public class CheckUserSessionMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                string? session = context.User.Claims.FirstOrDefault(c => c.Type == "SessionId")?.Value;
                if (string.IsNullOrWhiteSpace(session))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
                IMemoryCache cache = context.RequestServices.GetService<IMemoryCache>()!;
                if (!cache.TryGetValue(session, out _))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            await next.Invoke(context);
        }
    }
}
