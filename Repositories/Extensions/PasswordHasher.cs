using System.Security.Cryptography;

namespace Repositories.Extensions
{
    public static class PasswordHasher
    {
        // Generate salt (Base64 string)
        public static string GenerateSalt(int size = 16)
        {
            byte[] salt = new byte[size];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return Convert.ToBase64String(salt);
        }

        // Hash password with salt (Base64 string output)
        public static string HashPassword(string password, string base64Salt, int iterations = 100_000, int hashByteSize = 32)
        {
            byte[] salt = Convert.FromBase64String(base64Salt);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, iterations, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(hashByteSize);

            return Convert.ToBase64String(hash);
        }

        // Verify password
        public static bool VerifyPassword(string enteredPassword, string storedBase64Salt, string storedBase64Hash)
        {
            string enteredHash = HashPassword(enteredPassword, storedBase64Salt);
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromBase64String(storedBase64Hash),
                Convert.FromBase64String(enteredHash)
            );
        }
    }
}
