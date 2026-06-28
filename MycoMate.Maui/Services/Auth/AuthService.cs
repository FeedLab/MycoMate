using MycoMate.Maui.Api;
using Refit;

namespace MycoMate.Maui.Services.Auth;

public class AuthService(IMycoMateApiv1 api, CredentialStore credentialStore, TokenStore tokenStore)
{
    public async Task RegisterAsync(string email, string password)
    {
        try
        {
            await api.Register(new RegisterRequest { Email = email, Password = password });
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Registration failed: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task LoginAsync(string email, string password)
    {
        try
        {
            var response = await api.Login(new LoginRequest { Email = email, Password = password });
            tokenStore.Set(response.AccessToken, response.RefreshToken);
            await credentialStore.SaveAsync(email, password);
        }
        catch (ApiException ex)
        {
            throw new AuthException($"Login failed: {ex.ReasonPhrase}", ex);
        }
    }

}
