using MycoMate.Api.Models;

namespace MycoMate.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(MycoMateDbContext db)
    {
        if (db.Ingredients.Any())
            return;

        var ingredients = new List<Ingredient>
        {
            new() { ShortName = "WS",  DisplayName = "Wheat Straw",     MoistureContent = 10.0m, UserId = "system" },
            new() { ShortName = "HB",  DisplayName = "Hardwood Bran",   MoistureContent = 12.0m, UserId = "system" },
            new() { ShortName = "WB",  DisplayName = "Wheat Bran",      MoistureContent = 11.5m, UserId = "system" },
            new() { ShortName = "RB",  DisplayName = "Rice Bran",       MoistureContent =  9.5m, UserId = "system" },
            new() { ShortName = "OAT", DisplayName = "Oat Bran",        MoistureContent = 10.5m, UserId = "system" },
        };

        db.Ingredients.AddRange(ingredients);
        await db.SaveChangesAsync();
    }
}
