using System.Security.Claims;

namespace CustomExtensions;

public static class ClaimsPrincipalExtensions
{
    public static bool GetUserId(this ClaimsPrincipal claims, out int id)
    {
        return int.TryParse(claims.Claims.FirstOrDefault(c => c.Type == "Id")?.Value, out id);
    }
    public static string? GetUsername(this ClaimsPrincipal claims)
    {
        return claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}