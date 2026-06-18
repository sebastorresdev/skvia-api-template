using System.Security.Cryptography;

namespace SkviaApiTemplate.WebApi.Auth.Services;

public class PasswordService
{
    private const int SaltSize = 16;
    private const int HashSize = 32;
    private const int Iterations = 100_000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    public string Hash(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return $"{Convert.ToHexString(hash)}-{Convert.ToHexString(salt)}";
    }

    public bool Verificar(string password, string passwordHash)
    {
        var partes = passwordHash.Split('-');
        var hash = Convert.FromHexString(partes[0]);
        var salt = Convert.FromHexString(partes[1]);

        var hashNuevo = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, HashSize);

        return CryptographicOperations.FixedTimeEquals(hash, hashNuevo);
    }
}
