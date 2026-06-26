using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface IIngredientRepository
{
    Task<Ingredient> AddAsync(Ingredient ingredient, string userId, CancellationToken ct = default);
}
