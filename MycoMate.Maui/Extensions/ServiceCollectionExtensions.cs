using MycoMate.Maui.Api;
using MycoMate.Maui.Services.Auth;
using MycoMate.Maui.Services.Ingredients;
using MycoMate.Maui.Services.Projects;
using Refit;

namespace MycoMate.Maui.Extensions;

internal static class ServiceCollectionExtensions
{
    internal static IServiceCollection AddMycoMateApiClient(this IServiceCollection services, string baseAddress)
    {
        services
            .AddRefitClient<IMycoMateApiv1>()
            .ConfigureHttpClient(c =>
            {
                c.BaseAddress = new Uri(baseAddress);
                c.Timeout = TimeSpan.FromSeconds(30);
            })
            .AddStandardResilienceHandler(options =>
            {
                options.Retry.MaxRetryAttempts = 3;
                options.Retry.Delay = TimeSpan.FromSeconds(1);
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
                options.CircuitBreaker.FailureRatio = 0.5;
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(60);
            });

        services.AddSingleton<AuthService>();
        services.AddSingleton<IngredientService>();
        services.AddSingleton<ProjectService>();

        return services;
    }
}
