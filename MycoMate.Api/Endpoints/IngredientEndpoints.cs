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
        var group = app.MapGroup("/projects/{projectId:guid}/ingredients")
            .WithTags("Ingredients")
            .RequireAuthorization();

        group.MapGet("/", async (Guid projectId, ClaimsPrincipal user, IProjectRepository projectRepo,
                IIngredientRepository ingredientRepo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var project = await projectRepo.GetByIdAsync(projectId, ct);
                var role = await projectRepo.GetUserRoleAsync(projectId, userId, ct);

                if (project is null || role is null)
                {
                    return Results.NotFound();
                }

                var ingredients = await ingredientRepo.GetVisibleAsync(project.OwnerId, ct);
                var response = ingredients.Select(i => new IngredientResponse(
                    i.Id, i.ShortName, i.DisplayName, i.Information, i.MoistureContent, i.Created));

                return Results.Ok(response);
            })
            .WithName("GetIngredients");

        group.MapPost("/", async (Guid projectId, CreateIngredientRequest req, ClaimsPrincipal user,
                IProjectRepository projectRepo, IIngredientRepository ingredientRepo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await projectRepo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null)
                {
                    return Results.NotFound();
                }

                if (role < ProjectRole.Editor)
                {
                    return Results.Forbid();
                }

                var project = await projectRepo.GetByIdAsync(projectId, ct);

                var ingredient = new Ingredient
                {
                    ShortName       = req.ShortName,
                    DisplayName     = req.DisplayName,
                    Information     = req.Information,
                    MoistureContent = req.MoistureContent,
                    UserId          = project!.OwnerId
                };

                await ingredientRepo.AddAsync(ingredient, ct);

                var response = new IngredientResponse(
                    ingredient.Id, ingredient.ShortName, ingredient.DisplayName,
                    ingredient.Information, ingredient.MoistureContent, ingredient.Created);

                return Results.Created($"/projects/{projectId}/ingredients/{ingredient.Id}", response);
            })
            .WithName("CreateIngredient");

        return app;
    }
}
