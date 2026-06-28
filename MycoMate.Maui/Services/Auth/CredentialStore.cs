namespace MycoMate.Maui.Services.Auth;

public class CredentialStore
{
    private const string EmailKey = "auth_email";
    private const string PasswordKey = "auth_password";

    public string? SavedEmail => Preferences.Default.Get<string?>(EmailKey, null);

    public async Task SaveAsync(string email, string password)
    {
        Preferences.Default.Set(EmailKey, email);
        await SecureStorage.Default.SetAsync(PasswordKey, password);
    }

    public async Task<string?> GetPasswordAsync()
    {
        return await SecureStorage.Default.GetAsync(PasswordKey);
    }

    public void Clear()
    {
        Preferences.Default.Remove(EmailKey);
        SecureStorage.Default.Remove(PasswordKey);
    }
}
