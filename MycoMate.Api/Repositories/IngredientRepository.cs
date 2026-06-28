using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class IngredientRepository(MycoMateDbContext db, ILogger<IngredientRepository> logger) : IIngredientRepository
{
    public async Task<IEnumerable<Ingredient>> GetVisibleAsync(string ownerUserId, CancellationToken ct = default)
    {
        return await db.Ingredients
            .Where(i => i.UserId == "system" || i.UserId == ownerUserId)
            .ToListAsync(ct);
    }

    public async Task<Ingredient> AddAsync(Ingredient ingredient, CancellationToken ct = default)
    {
        logger.LogInformation("Adding ingredient {ShortName} ({DisplayName}) for user {UserId}", ingredient.ShortName, ingredient.DisplayName, ingredient.UserId);

        db.Ingredients.Add(ingredient);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Ingredient {Id} saved successfully", ingredient.Id);

        return ingredient;
    }
}
