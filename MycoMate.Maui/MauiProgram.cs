using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using MycoMate.Maui.Extensions;
using MycoMate.Maui.Services;
using Serilog;
using Syncfusion.Maui.Core.Hosting;

namespace MycoMate.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(
            "Ngo9BigBOggjHTQxAR8/V1JHaF5cWWdCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdlWXxccXZVQ2FeVEV/XUdWYEo=");

        Console.WriteLine($@"Idiom: {DeviceInfo.Idiom}");

        // Configure Serilog
        var memorySink = new MemoryLogSink();
        var logPath = Path.Combine(FileSystem.AppDataDirectory, "logs", "MycoMate.log");
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File(logPath, rollingInterval: RollingInterval.Day, retainedFileCountLimit: 7)
            .WriteTo.Sink(memorySink)
            .CreateLogger();

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureSyncfusionCore()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if ANDROID
        builder.Services.AddMycoMateApiClient("https://10.0.2.2:7010"); // Android emulator → host machine
#else
        builder.Services.AddMycoMateApiClient("https://localhost:7010");
#endif

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}