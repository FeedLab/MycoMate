using System.Text;
using System.Text.Json;

namespace MycoMate.Maui.Services.Auth;

public class TokenStore
{
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }
    public string? UserId { get; private set; }

    public void Set(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        UserId = ParseUserIdFromJwt(accessToken);
    }

    public void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
        UserId = null;
    }

    private static string? ParseUserIdFromJwt(string token)
    {
        try
        {
            var parts = token.Split('.');
            if (parts.Length != 3) return null;

            var payload = parts[1].Replace('-', '+').Replace('_', '/');
            payload += new string('=', (4 - payload.Length % 4) % 4);

            using var doc = JsonDocument.Parse(Encoding.UTF8.GetString(Convert.FromBase64String(payload)));

            // JwtSecurityTokenHandler maps ClaimTypes.NameIdentifier → "nameid"
            foreach (var key in new[] { "nameid", "sub" })
                if (doc.RootElement.TryGetProperty(key, out var el))
                    return el.GetString();

            return null;
        }
        catch { return null; }
    }
}
