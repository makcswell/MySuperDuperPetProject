using MySuperDuperPetProject.Middle;

namespace MySuperDuperPetProject.Middlewares
{

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
}

