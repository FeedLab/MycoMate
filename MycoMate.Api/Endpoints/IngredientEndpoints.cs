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
                    return Results.NotFound();

                var ingredients = await ingredientRepo.GetVisibleAsync(project.OwnerId, ct);
                var response = ingredients.Select(ToResponse);

                return Results.Ok(response);
            })
            .WithName("GetIngredients")
            .Produces<IEnumerable<IngredientResponse>>();

        group.MapPost("/", async (Guid projectId, CreateIngredientRequest req, ClaimsPrincipal user,
                IProjectRepository projectRepo, IIngredientRepository ingredientRepo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await projectRepo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null)
                    return Results.NotFound();

                if (role < ProjectRole.Editor)
                    return Results.Forbid();

                var project = await projectRepo.GetByIdAsync(projectId, ct);

                var ingredient = new Ingredient
                {
                    ShortName             = req.ShortName,
                    DisplayName           = req.DisplayName,
                    Information           = req.Information,
                    MoistureContent       = req.MoistureContent,
                    CarbonToNitrogenRatio = req.CarbonToNitrogenRatio,
                    PhLevel               = req.PhLevel,
                    Function              = req.Function,
                    BulkDensityKgPerM3    = req.BulkDensityKgPerM3,
                    UserId                = project!.OwnerId
                };

                if (req.Minerals != null)
                    foreach (var m in req.Minerals)
                        ingredient.Minerals.Add(new IngredientMineral { MineralId = m.NutrientId, Value = m.Value });

                if (req.Vitamins != null)
                    foreach (var v in req.Vitamins)
                        ingredient.Vitamins.Add(new IngredientVitamin { VitaminId = v.NutrientId, Value = v.Value });

                if (req.AminoAcids != null)
                    foreach (var a in req.AminoAcids)
                        ingredient.AminoAcids.Add(new IngredientAminoAcid { AminoAcidId = a.NutrientId, Value = a.Value });

                await ingredientRepo.AddAsync(ingredient, ct);

                return Results.Created($"/projects/{projectId}/ingredients/{ingredient.Id}", ToResponse(ingredient));
            })
            .WithName("CreateIngredient")
            .Produces<IngredientResponse>(StatusCodes.Status201Created);

        return app;
    }

    private static IngredientResponse ToResponse(Ingredient i) => new(
        i.Id, i.ShortName, i.DisplayName, i.Information, i.MoistureContent,
        i.CarbonToNitrogenRatio, i.PhLevel, i.Function, i.BulkDensityKgPerM3,
        i.Minerals.Select(m => new NutrientValueResponse(m.MineralId, m.Mineral.Name, m.Mineral.ShortName, m.Mineral.Description, m.Value, m.Mineral.Unit)).ToList(),
        i.Vitamins.Select(v => new NutrientValueResponse(v.VitaminId, v.Vitamin.Name, v.Vitamin.ShortName, v.Vitamin.Description, v.Value, v.Vitamin.Unit)).ToList(),
        i.AminoAcids.Select(a => new NutrientValueResponse(a.AminoAcidId, a.AminoAcid.Name, a.AminoAcid.ShortName, a.AminoAcid.Description, a.Value, a.AminoAcid.Unit)).ToList(),
        i.Created);
}
