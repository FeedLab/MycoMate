using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace MycoMate.Maui;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            if (e.ExceptionObject is Exception ex)
            {
                Log.Fatal(ex, "Unhandled AppDomain exception: {Message}", ex.Message);
            }
        };

        TaskScheduler.UnobservedTaskException += (s, e) =>
        {
            Log.Fatal(e.Exception, "Unobserved Task exception: {Message}", e.Exception.Message);
            e.SetObserved();
        };
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        var window = new Window(new AppShell());

        if (DeviceInfo.Platform == DevicePlatform.WinUI ||
            DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            var scale = 0.4;
            window.Width = 1290 * scale;
            window.Height = 2796 * scale;
        }
              
        return window;
    }
}
