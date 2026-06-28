namespace MycoMate.Maui.Views.Auth;

public partial class LoginView : ContentView
{
    public event EventHandler<(string Email, string Password)>? LoginRequested;
    public event EventHandler? ForgotPasswordRequested;

    public LoginView()
    {
        InitializeComponent();
    }

    public void Prefill(string email, string? password)
    {
        EmailEntry.Text = email;
        PasswordEntry.Text = password;
    }

    private void OnLoginClicked(object sender, EventArgs e)
    {
        LoginRequested?.Invoke(this, (EmailEntry.Text ?? string.Empty, PasswordEntry.Text ?? string.Empty));
    }

    private void OnForgotPasswordTapped(object sender, TappedEventArgs e)
    {
        ForgotPasswordRequested?.Invoke(this, EventArgs.Empty);
    }
}
