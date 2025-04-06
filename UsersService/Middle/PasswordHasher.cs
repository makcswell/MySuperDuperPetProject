using System.Security.Cryptography;
using System.Text;

namespace UsersService.Middle
{
    public class PasswordHasher
    {
        public static string HashPassword(string pas)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(pas, nameof(pas));

            byte[] hash = SHA512.HashData(Encoding.Default.GetBytes(pas));
            return Convert.ToBase64String(hash);
        }
    }
}
