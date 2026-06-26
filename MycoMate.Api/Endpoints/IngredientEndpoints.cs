using System.Security.Claims;
using MycoMate.Api.Contracts.Requests;
using MycoMate.Api.Contracts.Responses;
using MycoMate.Api.Models;
using MycoMate.Api.Repositories;

namespace MycoMate.Api.Endpoints;

public static class IngredientEndpoints
{
    public static IEndpointRouteBuilder MapIngredientEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/ingredients")
            .WithTags("Ingredients")
            .RequireAuthorization();

        group.MapPost("/", async (CreateIngredientRequest req, ClaimsPrincipal user, IIngredientRepository repo, CancellationToken ct) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var ingredient = new Ingredient
            {
                ShortName = req.ShortName,
                DisplayName = req.DisplayName,
                Information = req.Information,
                MoistureContent = req.MoistureContent,
                UserId = userId
            };

            await repo.AddAsync(ingredient, userId, ct);

            var response = new IngredientResponse(
                ingredient.Id,
                ingredient.ShortName,
                ingredient.DisplayName,
                ingredient.Information,
                ingredient.MoistureContent,
                ingredient.Created
            );

            return Results.Created($"/ingredients/{ingredient.Id}", response);
        })
        .WithName("CreateIngredient");

        return app;
    }
}
