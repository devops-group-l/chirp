using System.Security.Cryptography;
using System.Text;

namespace Chirp.WebService.Extensions;

public static class HttpExtensions
{
    public static string GetPathUrl(this HttpRequest request)
    {
        var origin = request.Headers["Origin"].ToString();
        var referer = request.Headers["Referer"].ToString();

        return referer.Replace(origin, "");
    }
    
    public static string SimpleHashPassword(this string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashBytes);
        }
    }
}
