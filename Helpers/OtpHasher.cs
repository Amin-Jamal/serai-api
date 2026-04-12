using System.Security.Cryptography;
using System.Text;

namespace Serai.AuthApi.Helpers;

public static class OtpHasher
{
    public static string Hash(string value, string secret)
    {
        var bytes = Encoding.UTF8.GetBytes($"{value}:{secret}");
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash);
    }
}
