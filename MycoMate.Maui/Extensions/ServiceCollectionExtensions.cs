using MycoMate.Maui.Api;
using MycoMate.Maui.PopupView;
using MycoMate.Maui.Services.Auth;
using MycoMate.Maui.Services.Ingredients;
using MycoMate.Maui.Services.Projects;
using MycoMate.Maui.Services.SubstrateRecipes;
using MycoMate.Maui.ViewModels;
using Refit;

namespace MycoMate.Maui.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddMycoMateApiClient(this IServiceCollection services, string baseAddress)
    {
        // Dedicated client for token refresh — no AuthHeaderHandler in its pipeline,
        // which breaks the circular DI dependency.
        var authClientBuilder = services.AddHttpClient("auth", c =>
        {
            c.BaseAddress = new Uri(baseAddress);
            c.Timeout = TimeSpan.FromSeconds(30);
        });

#if DEBUG
        authClientBuilder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });
#endif

        void ConfigureClient(IHttpClientBuilder builder)
        {
#if DEBUG
            builder.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            });
#endif
            builder.AddHttpMessageHandler<AuthHeaderHandler>();
            builder.AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
            });
        }

        var clientBuilder = services
            .AddRefitClient<IMycoMateApiv1>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseAddress);
                c.Timeout = TimeSpan.FromSeconds(30);
            });
        ConfigureClient(clientBuilder);

        services.AddSingleton<TokenStore>();
        services.AddTransient<AuthHeaderHandler>();
        services.AddSingleton<CredentialStore>();
        services.AddSingleton<AuthService>();
        services.AddSingleton<IngredientService>();
        services.AddSingleton<ProjectService>();
        services.AddSingleton<SubstrateRecipeService>();
        services.AddTransient<AuthPopup>();
        services.AddTransient<ProjectsViewModel>();

        return services;
    }
}
