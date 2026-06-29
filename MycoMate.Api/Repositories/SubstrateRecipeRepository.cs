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

    public async Task<bool> AddOrUpdateIngredientAsync(Guid recipeId, Guid ingredientId, decimal amount, CancellationToken ct = default)
    {
        var recipeExists = await db.SubstrateRecipes.AnyAsync(r => r.Id == recipeId, ct);

        if (!recipeExists)
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
            existing.WetAmount = amount;
            existing.MoistureContent = ingredient.MoistureContent;
        }
        else
        {
            db.RecipeIngredients.Add(new RecipeIngredient
            {
                RecipeId        = recipeId,
                IngredientId    = ingredientId,
                WetAmount       = amount,
                MoistureContent = ingredient.MoistureContent
            });
        }

        await db.SaveChangesAsync(ct);

        await RecalculateWetAmountPercentsAsync(recipeId, ct);

        return true;
    }

    public async Task<bool> RemoveIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken ct = default)
    {
        var rows = await db.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId)
            .ExecuteDeleteAsync(ct);

        if (rows > 0)
        {
            await RecalculateWetAmountPercentsAsync(recipeId, ct);
        }

        return rows > 0;
    }

    private async Task RecalculateWetAmountPercentsAsync(Guid recipeId, CancellationToken ct)
    {
        var recipe = await db.SubstrateRecipes.FindAsync([recipeId], ct);

        if (recipe is null)
        {
            return;
        }

        var all = await db.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId)
            .ToListAsync(ct);

        var totalWet = all.Sum(ri => ri.WetAmount);

        if (totalWet == 0)
        {
            return;
        }

        foreach (var ri in all)
        {
            ri.WetAmountPercent = Math.Round(ri.WetAmount / totalWet * 100m, 4);
            ri.WetMatter        = Math.Round(recipe.FinalMixtureSizeKg * (ri.WetAmountPercent / 100m), 3);
            ri.DryMatter        = Math.Round(ri.WetMatter * (1m - ri.MoistureContent / 100m), 3);
        }

        var totalDry = all.Sum(ri => ri.DryMatter);

        if (totalDry > 0)
        {
            foreach (var ri in all)
            {
                ri.DryAmountPercent = Math.Round(ri.DryMatter / totalDry * 100m, 4);
            }
        }

        var currentWaterKg = recipe.FinalMixtureSizeKg - totalDry;
        var targetWaterKg  = recipe.FinalMixtureSizeKg * (recipe.MoistureContentTarget / 100m);

        recipe.WaterAdjustmentPercent = recipe.FinalMixtureSizeKg > 0
            ? Math.Round((targetWaterKg - currentWaterKg) / recipe.FinalMixtureSizeKg * 100m, 4)
            : 0m;

        await db.SaveChangesAsync(ct);
    }
}
