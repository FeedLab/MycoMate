using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using MycoMate.Maui.PopupView;
using MycoMate.Maui.Services;

namespace MycoMate.Maui;

public partial class MainPage : ContentPage
{
    private bool _authShown;

    public MainPage()
    {
        InitializeComponent();

        Appearing += async (sender, args) =>
        {
            if (_authShown) return;
            _authShown = true;

            var popup = AppService.GetRequiredService<AuthPopup>();
            await this.ShowPopupAsync(popup);
        };
    }
}