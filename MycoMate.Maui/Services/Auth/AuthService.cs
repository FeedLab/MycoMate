using MycoMate.Maui.Api;
using Refit;

namespace MycoMate.Maui.Services.Auth;

public class AuthService(IMycoMateApiv1 api)
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
            await api.Login(new LoginRequest { Email = email, Password = password });
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Login failed: {ex.ReasonPhrase}", ex);
        }
    }
}
