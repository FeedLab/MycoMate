using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using MycoMate.Api.Auth;
using MycoMate.Api.Data;
using MycoMate.Api.Repositories;

namespace MycoMate.Api.Extensions;

public static class ServiceExtensions
{
    public static string GetJwtKey(IConfiguration config) =>
        config["Jwt:Key"] ?? Environment.GetEnvironmentVariable("JWT_KEY")
        ?? throw new InvalidOperationException("JWT key is not configured. Set 'Jwt:Key' in appsettings or the 'JWT_KEY' environment variable.");

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddIdentityCore<IdentityUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<MycoMateDbContext>();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = config["Jwt:Issuer"],
                    ValidAudience = config["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(GetJwtKey(config)))
                };
            });

        services.AddAuthorizationBuilder()
            .AddPolicy(Policies.CanCreateProject, p => p.RequireRole(Roles.Owner));

        services.AddScoped<IIngredientRepository, IngredientRepository>();
        services.AddScoped<IProjectRepository, ProjectRepository>();
        services.AddScoped<TokenService>();

        return services;
    }
}
