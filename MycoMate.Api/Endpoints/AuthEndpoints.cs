using Microsoft.AspNetCore.Identity;
using MycoMate.Api.Auth;
using MycoMate.Api.Contracts.Requests;
using MycoMate.Api.Contracts.Responses;

namespace MycoMate.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/register", async (RegisterRequest req, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) =>
        {
            if (!await roleManager.RoleExistsAsync(Roles.Owner))
                await roleManager.CreateAsync(new IdentityRole(Roles.Owner));

            var user = new IdentityUser { UserName = req.Email, Email = req.Email };
            var result = await userManager.CreateAsync(user, req.Password);

            if (!result.Succeeded)
                return Results.ValidationProblem(
                    result.Errors.ToDictionary(e => e.Code, e => new[] { e.Description }));

            await userManager.AddToRoleAsync(user, Roles.Owner);

            return Results.Ok();
        })
        .WithName("Register")
        .WithTags("Auth")
        .AllowAnonymous();

        app.MapPost("/login", async (LoginRequest req, UserManager<IdentityUser> userManager, TokenService tokenService) =>
        {
            var user = await userManager.FindByEmailAsync(req.Email);
            if (user is null || !await userManager.CheckPasswordAsync(user, req.Password))
                return Results.Unauthorized();

            var token = await tokenService.GenerateTokenAsync(user);
            return Results.Ok(new TokenResponse(token));
        })
        .WithName("Login")
        .WithTags("Auth")
        .AllowAnonymous();

        // OAuth2 password grant — used by Scalar to auto-fetch and store tokens
        app.MapPost("/connect/token", async (HttpRequest request, UserManager<IdentityUser> userManager, TokenService tokenService) =>
        {
            if (!request.HasFormContentType)
                return Results.BadRequest(new { error = "invalid_request" });

            var form = await request.ReadFormAsync();

            if (form["grant_type"] != "password")
                return Results.BadRequest(new { error = "unsupported_grant_type" });

            var username = form["username"].ToString();
            var password = form["password"].ToString();

            var user = await userManager.FindByEmailAsync(username);
            if (user is null || !await userManager.CheckPasswordAsync(user, password))
                return Results.Json(new { error = "invalid_grant" }, statusCode: StatusCodes.Status400BadRequest);

            var token = await tokenService.GenerateTokenAsync(user);
            return Results.Ok(new { access_token = token, token_type = "Bearer", expires_in = 3600 });
        })
        .WithName("ConnectToken")
        .WithTags("Auth")
        .AllowAnonymous()
        .ExcludeFromDescription();

        return app;
    }
}
