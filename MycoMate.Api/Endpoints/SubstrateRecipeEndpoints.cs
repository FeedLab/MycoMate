using System.Security.Claims;
using MycoMate.Api.Contracts.Requests;
using MycoMate.Api.Contracts.Responses;
using MycoMate.Api.Models;
using MycoMate.Api.Repositories;

namespace MycoMate.Api.Endpoints;

public static class SubstrateRecipeEndpoints
{
    public static IEndpointRouteBuilder MapSubstrateRecipeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects/{projectId:guid}/recipes")
            .WithTags("SubstrateRecipes")
            .RequireAuthorization();

        group.MapGet("/", async (Guid projectId, ClaimsPrincipal user, IProjectRepository projectRepo,
                ISubstrateRecipeRepository recipeRepo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await projectRepo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null)
                {
                    return Results.NotFound();
                }

                var recipes = await recipeRepo.GetAllAsync(projectId, ct);
                var response = recipes.Select(ToResponse);

                return Results.Ok(response);
            })
            .WithName("GetSubstrateRecipes")
            .Produces<IEnumerable<SubstrateRecipeResponse>>();

        group.MapGet("/{id:guid}", async (Guid projectId, Guid id, ClaimsPrincipal user,
                IProjectRepository projectRepo, ISubstrateRecipeRepository recipeRepo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await projectRepo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null)
                {
                    return Results.NotFound();
                }

                var recipe = await recipeRepo.GetByIdAsync(projectId, id, ct);

                if (recipe is null)
                {
                    return Results.NotFound();
                }

                return Results.Ok(ToResponse(recipe));
            })
            .WithName("GetSubstrateRecipe")
            .Produces<SubstrateRecipeResponse>();

        group.MapPost("/", async (Guid projectId, CreateSubstrateRecipeRequest req, ClaimsPrincipal user,
                IProjectRepository projectRepo, ISubstrateRecipeRepository recipeRepo, CancellationToken ct) =>
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

                var recipe = new SubstrateRecipe
                {
                    Name                  = req.Name,
                    Description           = req.Description,
                    MoistureContentTarget = req.MoistureContentTarget,
                    FinalMixtureSizeKg    = req.FinalMixtureSizeKg,
                    ProjectId             = projectId
                };

                await recipeRepo.AddAsync(recipe, ct);

                return Results.Created($"/projects/{projectId}/recipes/{recipe.Id}", ToResponse(recipe));
            })
            .WithName("CreateSubstrateRecipe")
            .Produces<SubstrateRecipeResponse>(StatusCodes.Status201Created);

        group.MapPut("/{id:guid}", async (Guid projectId, Guid id, UpdateSubstrateRecipeRequest req,
                ClaimsPrincipal user, IProjectRepository projectRepo, ISubstrateRecipeRepository recipeRepo,
                CancellationToken ct) =>
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

                var recipe = new SubstrateRecipe
                {
                    Id                    = id,
                    Name                  = req.Name,
                    Description           = req.Description,
                    MoistureContentTarget = req.MoistureContentTarget,
                    FinalMixtureSizeKg    = req.FinalMixtureSizeKg,
                    ProjectId             = projectId
                };

                var updated = await recipeRepo.UpdateAsync(recipe, ct);

                if (!updated)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            })
            .WithName("UpdateSubstrateRecipe");

        group.MapDelete("/{id:guid}", async (Guid projectId, Guid id, ClaimsPrincipal user,
                IProjectRepository projectRepo, ISubstrateRecipeRepository recipeRepo, CancellationToken ct) =>
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

                var deleted = await recipeRepo.DeleteAsync(projectId, id, ct);

                if (!deleted)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            })
            .WithName("DeleteSubstrateRecipe");

        // Ingredient management
        group.MapPut("/{id:guid}/ingredients/{ingredientId:guid}", async (Guid projectId, Guid id, Guid ingredientId,
                RecipeIngredientRequest req, ClaimsPrincipal user, IProjectRepository projectRepo,
                ISubstrateRecipeRepository recipeRepo, CancellationToken ct) =>
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

                var ok = await recipeRepo.AddOrUpdateIngredientAsync(id, ingredientId, req.DryPercent, ct);

                if (!ok)
                {
                    return Results.NotFound();
                }

                var recipe = await recipeRepo.GetByIdAsync(projectId, id, ct);

                return Results.Ok(ToResponse(recipe!));
            })
            .WithName("SetRecipeIngredient")
            .Produces<SubstrateRecipeResponse>();

        group.MapDelete("/{id:guid}/ingredients/{ingredientId:guid}", async (Guid projectId, Guid id,
                Guid ingredientId, ClaimsPrincipal user, IProjectRepository projectRepo,
                ISubstrateRecipeRepository recipeRepo, CancellationToken ct) =>
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

                var removed = await recipeRepo.RemoveIngredientAsync(id, ingredientId, ct);

                if (!removed)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            })
            .WithName("RemoveRecipeIngredient");

        return app;
    }

    private static SubstrateRecipeResponse ToResponse(SubstrateRecipe r) =>
        new(r.Id, r.Name, r.Description, r.MoistureContentTarget, r.FinalMixtureSizeKg, r.WaterAdjustmentPercent, r.Created, r.ProjectId,
            r.Ingredients.Select(ri => new RecipeIngredientResponse(
                ri.IngredientId, ri.Ingredient?.ShortName ?? "", ri.Ingredient?.DisplayName ?? "",
                ri.MoistureContent, ri.DryPercent)));
}
