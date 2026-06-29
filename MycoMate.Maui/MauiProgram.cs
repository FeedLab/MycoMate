using System.Reflection;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Configuration;
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

        var assembly = Assembly.GetExecutingAssembly();

        using (var stream = assembly.GetManifestResourceStream("MycoMate.Maui.appsettings.json"))
            builder.Configuration.AddJsonStream(stream!);

#if DEBUG
        using (var stream = assembly.GetManifestResourceStream("MycoMate.Maui.appsettings.Development.json"))
            if (stream is not null)
                builder.Configuration.AddJsonStream(stream);
#endif

#if ANDROID && DEBUG
        var baseAddress = builder.Configuration["Api:AndroidBaseUrl"]!;
#else
        var baseAddress = builder.Configuration["Api:BaseUrl"]!;
#endif

        builder.Services.AddMycoMateApiClient(baseAddress);

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}