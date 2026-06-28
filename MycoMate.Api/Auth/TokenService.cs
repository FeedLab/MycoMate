using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MycoMate.Api.Contracts.Responses;
using MycoMate.Api.Extensions;

namespace MycoMate.Api.Auth;

public class TokenService(IConfiguration config, UserManager<IdentityUser> userManager)
{
    private const string LoginProvider        = "MycoMate";
    private const string RefreshTokenName     = "RefreshToken";
    private const string RefreshTokenExpiry   = "RefreshTokenExpiry";

    private static readonly TimeSpan RefreshTokenLifetime = TimeSpan.FromDays(30);

    public async Task<TokenResponse> GenerateTokenPairAsync(IdentityUser user)
    {
        var accessToken  = await GenerateAccessTokenAsync(user);
        var refreshToken = GenerateRefreshToken();
        var expiry       = DateTime.UtcNow.Add(RefreshTokenLifetime).ToString("O");

        await userManager.SetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName, refreshToken);
        await userManager.SetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenExpiry, expiry);

        return new TokenResponse(accessToken, refreshToken);
    }

    public async Task<IdentityUser?> ValidateRefreshTokenAsync(string refreshToken)
    {
        var users = userManager.Users.ToList();

        foreach (var user in users)
        {
            var stored = await userManager.GetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenName);

            if (stored != refreshToken)
            {
                continue;
            }

            var expiryRaw = await userManager.GetAuthenticationTokenAsync(user, LoginProvider, RefreshTokenExpiry);

            if (expiryRaw is null || DateTime.Parse(expiryRaw, null, System.Globalization.DateTimeStyles.RoundtripKind) < DateTime.UtcNow)
            {
                return null;
            }

            return user;
        }

        return null;
    }

    private async Task<string> GenerateAccessTokenAsync(IdentityUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
        };

        claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(ServiceExtensions.GetJwtKey(config)));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        var bytes = new byte[64];
        RandomNumberGenerator.Fill(bytes);
        return Convert.ToBase64String(bytes);
    }
}
