using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface IIngredientRepository
{
    Task<IEnumerable<Ingredient>> GetVisibleAsync(string ownerUserId, CancellationToken ct = default);
    Task<Ingredient> AddAsync(Ingredient ingredient, CancellationToken ct = default);
}
