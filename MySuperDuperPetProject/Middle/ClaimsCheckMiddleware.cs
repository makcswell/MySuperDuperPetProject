using System.IdentityModel.Tokens.Jwt;

namespace MySuperDuperPetProject.Middle
{
    
    public class ClaimsCheckMiddleware
    {
        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization",out var token))
            {
                var jwtToken = token.ToString().Split(" ").Last();
                var handler = new JwtSecurityTokenHandler();
                var jwtTokenObj = handler.ReadJwtToken(jwtToken);
                if (!jwtTokenObj.Claims.Any(c=>c.Type == "Constant")){
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unathorized:Missing required claim");
                    return;
                }


            }

        }
    }
}
