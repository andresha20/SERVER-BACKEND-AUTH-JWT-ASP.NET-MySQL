using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace APIToken.Validations
{
    public class Validator
    {
        private const int keySize = 64;
        private const int iterations = 350000;
        readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;
        public string EncodePassword(string password, out string salt)
        {
            if (password == null) { throw new ArgumentNullException(nameof(password)); };
            var saltByte = RandomNumberGenerator.GetBytes(keySize);
            salt = Convert.ToHexString(saltByte);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltByte,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }

        public bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }
    }
}
