using Microsoft.Extensions.Caching.Memory;


namespace MySuperDuperPetProject.Middlewares
{

    public class ClaimsCheckMiddleware : IMiddleware
    {
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {

            if (context.User?.Identity?.IsAuthenticated ?? false)
            {
                string? Constant = context.User.Claims?.FirstOrDefault(c => c.Type == "Constant")?.Value;
                if (string.IsNullOrWhiteSpace(Constant))
                {
                    context.Response.StatusCode = 401;
                    return;
                }

            }
            await next.Invoke(context);

        }

    }
}

