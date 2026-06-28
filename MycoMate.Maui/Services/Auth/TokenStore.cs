namespace MycoMate.Maui.Services.Auth;

public class TokenStore
{
    public string? AccessToken { get; private set; }
    public string? RefreshToken { get; private set; }

    public void Set(string accessToken, string refreshToken)
    {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }

    public void Clear()
    {
        AccessToken = null;
        RefreshToken = null;
    }
}
