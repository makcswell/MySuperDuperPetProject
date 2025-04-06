using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using StaticStorages;

namespace Middlewares;

public class ClaimsCheckMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {

        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            if (context.User.Claims?.FirstOrDefault
                (c => c.Type == CustomClaimTypesStorage.ConstantClaim.Type && 
                      c.Value == CustomClaimTypesStorage.ConstantClaim.Value) == null)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Please regenerate token!");
                return;
            }

        }
        await next.Invoke(context);
    }

}

public static class ClaimsCheckMiddlewareExtensions
{
    public static IServiceCollection AddClaimsCheckMiddleware(this IServiceCollection services)
    {
        return services.AddSingleton<ClaimsCheckMiddleware>();
    }
    public static IApplicationBuilder UseClaimsCheckMiddleware(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<ClaimsCheckMiddleware>();
    }
}