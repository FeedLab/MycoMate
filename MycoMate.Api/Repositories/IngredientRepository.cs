using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class IngredientRepository(MycoMateDbContext db, ILogger<IngredientRepository> logger) : IIngredientRepository
{
    public async Task<Ingredient> AddAsync(Ingredient ingredient, string userId, CancellationToken ct = default)
    {
        ingredient.UserId = userId;

        logger.LogInformation("Adding ingredient {ShortName} ({DisplayName}) for user {UserId}", ingredient.ShortName, ingredient.DisplayName, userId);

        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Ingredient {Id} saved successfully", ingredient.Id);

        return ingredient;
    }
}
