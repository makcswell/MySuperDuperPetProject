using Microsoft.Extensions.Caching.Memory;
using System.IdentityModel.Tokens.Jwt;
using static System.Net.Mime.MediaTypeNames;

namespace MySuperDuperPetProject.Middle
{
    
    public class ClaimsCheckMiddleware
    {
        public async Task Invoke(HttpContext context, RequestDelegate next)
        {
           
            if(context.User?.Identity?.IsAuthenticated ?? false)
            {
                string? Constant = context.User.Claims?.FirstOrDefault(c=> c.Type == "Constant")?.Value;
                if (string.IsNullOrWhiteSpace(Constant))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
                IMemoryCache cache = context.RequestServices.GetService<IMemoryCache>()!;
                if (!cache.TryGetValue(Constant, out _))
                {
                    context.Response.StatusCode = 401;
                    return;
                }
            }
            await next.Invoke(context);

        }

    }
    }

