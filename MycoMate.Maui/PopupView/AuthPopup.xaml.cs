using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MycoMate.Maui.Messages;
using MycoMate.Maui.Services.Auth;

namespace MycoMate.Maui.PopupView;

public partial class AuthPopup : Popup
{
    private readonly AuthService authService;
    private readonly CredentialStore credentialStore;
    private readonly ILogger<AuthPopup> logger;

    public AuthPopup(AuthService authService, CredentialStore credentialStore, ILogger<AuthPopup> logger)
    {
        InitializeComponent();
        
        this.authService = authService;
        this.credentialStore = credentialStore;
        this.logger = logger;

        Loaded += OnLoaded;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        var email = credentialStore.SavedEmail;
        
        if (email is null)
        {
            logger.LogDebug("No saved credentials found");
            return;
        }

        logger.LogDebug("Prefilling credentials for {Email}", email);
       
        var password = await credentialStore.GetPasswordAsync();
        
        LoginView.Prefill(email, password);
    }

    private async void OnLoginRequested(object? sender, (string Email, string Password) args)
    {
        logger.LogInformation("Login attempt for {Email}", args.Email);
       
        SetBusy(true);
        
        try
        {
            await authService.LoginAsync(args.Email, args.Password);
            logger.LogInformation("Login succeeded for {Email}", args.Email);
            
            await CloseAsync();
            
            WeakReferenceMessenger.Default.Send(new UserLoggedInMessage(args.Email));
        }
        catch (AuthException authException)
        {
            logger.LogWarning(authException, "Login failed for {Email}", args.Email);
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Login failed", authException.Message, "OK");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error during login for {Email}", args.Email);
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("General Error", ex.Message, "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private async void OnRegisterRequested(object? sender, (string Email, string Password) args)
    {
        logger.LogInformation("Register attempt for {Email}", args.Email);
        
        SetBusy(true);
        
        try
        {
            await authService.RegisterAsync(args.Email, args.Password);
            logger.LogInformation("Registration succeeded for {Email}", args.Email);
            await CloseAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Registration failed for {Email}", args.Email);
            await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Registration failed", ex.Message, "OK");
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void SetBusy(bool busy)
    {
        ContentGrid.IsEnabled = !busy;
        BusyOverlay.IsVisible = busy;
    }

    private void OnForgotPasswordRequested(object? sender, EventArgs e)
    {
        logger.LogDebug("Forgot password requested");
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        logger.LogDebug("Auth popup closed by user");
        
        await CloseAsync();
    }
}
