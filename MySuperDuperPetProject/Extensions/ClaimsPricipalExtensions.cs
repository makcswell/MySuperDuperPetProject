using System.Security.Claims;

namespace MySuperDuperPetProject.Extensions
{
    public static class ClaimsPricipalExtensions
    {
        public static bool GetUserId(this ClaimsPrincipal claims, out int id)
        {
            if (!int.TryParse(claims.Claims.FirstOrDefault(c => c.Type == "Id")?.Value, out id))
            {
                return false;
            }
            return true;
        }
        public static bool GetUsername(this ClaimsPrincipal claims, out string username)
        {
            string? stringValue = claims.Claims.FirstOrDefault(c=> c.Type== "username")?.Value;  
            if (string.IsNullOrEmpty(stringValue))
            {
                username = string.Empty;
                return false;
            }
            username = stringValue;
            return true;
        }
    }
}
