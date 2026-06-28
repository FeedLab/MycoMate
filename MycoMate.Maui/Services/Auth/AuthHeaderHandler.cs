using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MycoMate.Maui.Api;

namespace MycoMate.Maui.Services.Auth;

public class AuthHeaderHandler(TokenStore tokenStore, IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    private static readonly SemaphoreSlim refreshLock = new(1, 1);

    // Propagates down into the refresh HTTP call so the refresh response's own
    // 401 (invalid refresh token) doesn't trigger another refresh attempt.
    private static readonly AsyncLocal<bool> isRefreshing = new();

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var tokenOnSend = tokenStore.AccessToken;

        if (tokenOnSend is not null)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tokenOnSend);

        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized
            && !isRefreshing.Value
            && tokenStore.RefreshToken is not null)
        {
            var newToken = await TryRefreshAsync(tokenOnSend, cancellationToken);
            if (newToken is not null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", newToken);
                response.Dispose();
                response = await base.SendAsync(request, cancellationToken);
            }
        }

        return response;
    }

    private async Task<string?> TryRefreshAsync(string? tokenOnSend, CancellationToken cancellationToken)
    {
        await refreshLock.WaitAsync(cancellationToken);
        try
        {
            // Another concurrent request may have already refreshed while we waited.
            if (tokenStore.AccessToken != tokenOnSend)
                return tokenStore.AccessToken;

            isRefreshing.Value = true;
            return await RefreshAsync(cancellationToken);
        }
        finally
        {
            isRefreshing.Value = false;
            refreshLock.Release();
        }
    }

    private async Task<string?> RefreshAsync(CancellationToken cancellationToken)
    {
        var refreshToken = tokenStore.RefreshToken;
        if (refreshToken is null) return null;

        try
        {
            // Use the dedicated "auth" HttpClient — no AuthHeaderHandler in its pipeline,
            // which is what breaks the circular dependency.
            var client = httpClientFactory.CreateClient("auth");
            using var response = await client.PostAsJsonAsync(
                "/refresh", new RefreshRequest { RefreshToken = refreshToken }, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                tokenStore.Clear();
                return null;
            }

            var tokenPair = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: cancellationToken);
            if (tokenPair is null)
            {
                tokenStore.Clear();
                return null;
            }

            tokenStore.Set(tokenPair.AccessToken, tokenPair.RefreshToken);
            return tokenPair.AccessToken;
        }
        catch
        {
            tokenStore.Clear();
            return null;
        }
    }
}
