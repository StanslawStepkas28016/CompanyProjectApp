using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace CompanyProjectApp.Helpers;

public class SecurityHelper
{
    public static Tuple<string, string> GetHashedPasswordAndSalt(string password)
    {
        var salt = new byte[128 / 8];
        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(salt);
        }

        var hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        var saltBase64 = Convert.ToBase64String(salt);

        return new Tuple<string, string>(hashed, saltBase64);
    }

    public static string GetHashedPasswordWithSalt(string password, string salt)
    {
        var saltBytes = Convert.FromBase64String(salt);

        var currentHashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: saltBytes,
            prf: KeyDerivationPrf.HMACSHA1,
            iterationCount: 10000,
            numBytesRequested: 256 / 8));

        return currentHashedPassword;
    }

    public static string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var generator = RandomNumberGenerator.Create();
        generator.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}