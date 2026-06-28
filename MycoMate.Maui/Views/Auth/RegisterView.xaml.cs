namespace MycoMate.Maui.Views.Auth;

public partial class RegisterView : ContentView
{
    public event EventHandler<(string Email, string Password)>? RegisterRequested;

    public RegisterView()
    {
        InitializeComponent();
    }

    private void OnRegisterClicked(object sender, EventArgs e)
    {
        if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
        {
            // surface mismatch to user — can be replaced with a proper alert
            ConfirmPasswordEntry.Placeholder = "Passwords do not match";
            return;
        }

        RegisterRequested?.Invoke(this, (EmailEntry.Text ?? string.Empty, PasswordEntry.Text ?? string.Empty));
    }
}
