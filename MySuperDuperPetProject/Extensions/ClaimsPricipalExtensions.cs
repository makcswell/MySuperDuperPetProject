﻿using System.Security.Claims;

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
        public static string? GetUsername(this ClaimsPrincipal claims)
        {
            return claims.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
