using System.Security.Cryptography;
using System.Text;

namespace A_PRN212.Application.Common
{
    public static class PasswordHasher
    {
        private const int Sha256HexLength = 64;

        public static string Hash(string password)
        {
            ArgumentNullException.ThrowIfNull(password);
            byte[] hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        public static bool IsHashed(string? value)
        {
            if (string.IsNullOrEmpty(value) || value.Length != Sha256HexLength)
            {
                return false;
            }

            foreach (char c in value)
            {
                bool isHex = (c >= '0' && c <= '9') ||
                             (c >= 'a' && c <= 'f') ||
                             (c >= 'A' && c <= 'F');
                if (!isHex)
                {
                    return false;
                }
            }

            return true;
        }

        public static string EnsureHashed(string password)
        {
            return IsHashed(password) ? password.ToLowerInvariant() : Hash(password);
        }

        public static bool Verify(string inputPassword, string? storedPassword)
        {
            if (string.IsNullOrEmpty(storedPassword))
            {
                return false;
            }

            if (IsHashed(storedPassword))
            {
                byte[] inputHash = SHA256.HashData(Encoding.UTF8.GetBytes(inputPassword));
                byte[] storedHash = Convert.FromHexString(storedPassword);
                return CryptographicOperations.FixedTimeEquals(inputHash, storedHash);
            }

            // Backward compatibility for legacy plaintext rows before migration
            return string.Equals(inputPassword, storedPassword, StringComparison.Ordinal);
        }
    }
}
