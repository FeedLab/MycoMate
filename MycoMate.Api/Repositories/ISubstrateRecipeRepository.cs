using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface ISubstrateRecipeRepository
{
    Task<IEnumerable<SubstrateRecipe>> GetAllAsync(Guid projectId, CancellationToken ct = default);
    Task<SubstrateRecipe?> GetByIdAsync(Guid projectId, Guid id, CancellationToken ct = default);
    Task<SubstrateRecipe> AddAsync(SubstrateRecipe recipe, CancellationToken ct = default);
    Task<bool> UpdateAsync(SubstrateRecipe recipe, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid projectId, Guid id, CancellationToken ct = default);

    Task<bool> AddOrUpdateIngredientAsync(Guid recipeId, Guid ingredientId, decimal dryPercent, CancellationToken ct = default);
    Task<bool> RemoveIngredientAsync(Guid recipeId, Guid ingredientId, CancellationToken ct = default);
}
