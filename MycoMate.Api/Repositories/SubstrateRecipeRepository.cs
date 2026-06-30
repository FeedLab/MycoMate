using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class SubstrateRecipeRepository(MycoMateDbContext db) : ISubstrateRecipeRepository
{
    public async Task<IEnumerable<SubstrateRecipe>> GetAllAsync(Guid projectId, CancellationToken ct = default)
    {
        return await db.SubstrateRecipes
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .Where(r => r.ProjectId == projectId)
            .ToListAsync(ct);
    }

    public async Task<SubstrateRecipe?> GetByIdAsync(Guid projectId, Guid id, CancellationToken ct = default)
    {
        return await db.SubstrateRecipes
            .Include(r => r.Ingredients)
                .ThenInclude(ri => ri.Ingredient)
            .FirstOrDefaultAsync(r => r.ProjectId == projectId && r.Id == id, ct);
    }

    public async Task<SubstrateRecipe> AddAsync(SubstrateRecipe recipe, CancellationToken ct = default)
    {
        db.SubstrateRecipes.Add(recipe);

        await db.SaveChangesAsync(ct);

        return recipe;
    }

    public async Task<bool> UpdateAsync(SubstrateRecipe recipe, CancellationToken ct = default)
    {
        var rows = await db.SubstrateRecipes
            .Where(r => r.ProjectId == recipe.ProjectId && r.Id == recipe.Id)
            .ExecuteUpdateAsync(s => s
                .SetProperty(r => r.Name, recipe.Name)
                .SetProperty(r => r.Description, recipe.Description)
                .SetProperty(r => r.MoistureContentTarget, recipe.MoistureContentTarget)
                .SetProperty(r => r.FinalMixtureSizeKg, recipe.FinalMixtureSizeKg), ct);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid projectId, Guid id, CancellationToken ct = default)
    {
        var rows = await db.SubstrateRecipes
            .Where(r => r.ProjectId == projectId && r.Id == id)
            .ExecuteDeleteAsync(ct);

        return rows > 0;
    }

    public async Task<bool> AddOrUpdateIngredientAsync(Guid recipeId, Guid ingredientId, decimal dryPercent, CancellationToken ct = default)
    {
        var recipe = await db.SubstrateRecipes.FindAsync([recipeId], ct);

        if (recipe is null)
        {
            return false;
        }

        var ingredient = await db.Ingredients.FindAsync([ingredientId], ct);

        if (ingredient is null)
        {
            return false;
        }

        var existing = await db.RecipeIngredients
            .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId, ct);

        if (existing is not null)
        {
            existing.DryPercent      = dryPercent;
            existing.MoistureContent = ingredient.MoistureContent;
        }
        else
        {
            db.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId        = recipeId,
                IngredientId    = ingredientId,
                DryPercent      = dryPercent,
                MoistureContent = ingredient.MoistureContent
            });
        }

        await db.SaveChangesAsync(ct);

        await RecalculateWaterAdjustmentAsync(recipe, ct);

        return true;
    }

    public async Task<bool> RemoveIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken ct = default)
    {
        var rows = await db.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId)
            .ExecuteDeleteAsync(ct);

        if (rows > 0)
        {
            var recipe = await db.SubstrateRecipes.FindAsync([recipeId], ct);

            if (recipe is not null)
            {
                await RecalculateWaterAdjustmentAsync(recipe, ct);
            }
        }

        return rows > 0;
    }

    private async Task RecalculateWaterAdjustmentAsync(SubstrateRecipe recipe, CancellationToken ct)
    {
        if (recipe.FinalMixtureSizeKg == 0)
        {
            return;
        }

        var ingredients = await db.RecipeIngredients
            .Where(ri => ri.RecipeId == recipe.Id)
            .ToListAsync(ct);

        var totalDryKg = recipe.FinalMixtureSizeKg * (1m - recipe.MoistureContentTarget / 100m);

        var waterFromIngredients = ingredients.Sum(ri =>
        {
            if (ri.MoistureContent >= 100m || ri.DryPercent == 0m)
            {
                return 0m;
            }

            var dryKg = totalDryKg * (ri.DryPercent / 100m);
            var wetKg = dryKg / (1m - ri.MoistureContent / 100m);

            return wetKg - dryKg;
        });

        var targetWaterKg = recipe.FinalMixtureSizeKg * (recipe.MoistureContentTarget / 100m);

        recipe.WaterAdjustmentPercent = Math.Round(
            (targetWaterKg - waterFromIngredients) / recipe.FinalMixtureSizeKg * 100m, 4);

        await db.SaveChangesAsync(ct);
    }
}
