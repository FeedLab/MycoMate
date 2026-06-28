using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class SubstrateRecipeRepository(MycoMateDbContext db) : ISubstrateRecipeRepository
{
    public async Task<IEnumerable<SubstrateRecipe>> GetAllAsync(Guid projectId, CancellationToken ct = default)
    {
        return await db.SubstrateRecipes
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
        if (!recipeExists) return false;

        var existing = await db.RecipeIngredients
            .FirstOrDefaultAsync(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId, ct);

        if (existing is not null)
        {
            existing.Amount = amount;
        }
        else
        {
            db.RecipeIngredients.Add(new RecipeIngredient { RecipeId = recipeId, IngredientId = ingredientId, Amount = amount });
        }

        await db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken ct = default)
    {
        var rows = await db.RecipeIngredients
            .Where(ri => ri.RecipeId == recipeId && ri.IngredientId == ingredientId)
            .ExecuteDeleteAsync(ct);

        return rows > 0;
    }
}
