using System.Security.Cryptography;
using System.Text;

namespace MySuperDuperPetProject.Middle
{
    public class PasswordHasher
    {
        public static string HashPassword(string pas)
        {
            if (string.IsNullOrWhiteSpace(pas))
            {
                throw new ArgumentException($"\"{nameof(pas)}\" не может быть пустым или содержать только пробел.", nameof(pas));
            }

            byte[] hash = SHA512.HashData(Encoding.Default.GetBytes(pas));
            return Convert.ToBase64String(hash);
        }
    }
}
