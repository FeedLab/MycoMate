using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(MycoMateDbContext db, UserManager<IdentityUser> userManager)
    {
        await SeedUsersAsync(userManager);
        await SeedIngredientsAsync(db);
        await SeedProjectAsync(db, userManager);
    }

    private static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
    {
        var users = new[]
        {
            new { Email = "admin@mycomate.com",  Password = "Admin123$"  },
            new { Email = "user@mycomate.com",   Password = "User123$"   },
            new { Email = "reader@mycomate.com", Password = "Reader123$" },
        };

        foreach (var u in users)
        {
            if (await userManager.FindByEmailAsync(u.Email) is not null)
                continue;

            var identityUser = new IdentityUser { UserName = u.Email, Email = u.Email, EmailConfirmed = true };
            await userManager.CreateAsync(identityUser, u.Password);
        }
    }

    private static async Task SeedIngredientsAsync(MycoMateDbContext db)
    {
        if (await db.Ingredients.AnyAsync())
            return;

        db.Ingredients.AddRange(
            new Ingredient { ShortName = "WS",  DisplayName = "Wheat Straw",   MoistureContent = 10.0m, UserId = "system" },
            new Ingredient { ShortName = "HB",  DisplayName = "Hardwood Bran", MoistureContent = 12.0m, UserId = "system" },
            new Ingredient { ShortName = "WB",  DisplayName = "Wheat Bran",    MoistureContent = 11.5m, UserId = "system" },
            new Ingredient { ShortName = "RB",  DisplayName = "Rice Bran",     MoistureContent =  9.5m, UserId = "system" },
            new Ingredient { ShortName = "OAT", DisplayName = "Oat Bran",      MoistureContent = 10.5m, UserId = "system" },
            new Ingredient { ShortName = "H2O", DisplayName = "Water",         MoistureContent = 100m,  UserId = "system" }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedProjectAsync(MycoMateDbContext db, UserManager<IdentityUser> userManager)
    {
        if (await db.Projects.AnyAsync())
            return;

        var admin  = await userManager.FindByEmailAsync("admin@mycomate.com");
        var editor = await userManager.FindByEmailAsync("user@mycomate.com");
        var reader = await userManager.FindByEmailAsync("reader@mycomate.com");

        if (admin is null || editor is null || reader is null)
            return;

        var project = new Project { Name = "Sample Project", OwnerId = admin.Id };
        project.Members.Add(new ProjectMember { UserId = admin.Id,  Role = ProjectRole.Owner    });
        project.Members.Add(new ProjectMember { UserId = editor.Id, Role = ProjectRole.Editor   });
        project.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.ReadOnly });
        db.Projects.Add(project);

        var ws  = await db.Ingredients.FirstAsync(i => i.ShortName == "WS");
        var wb  = await db.Ingredients.FirstAsync(i => i.ShortName == "WB");
        var oat = await db.Ingredients.FirstAsync(i => i.ShortName == "OAT");

        var recipe = new SubstrateRecipe
        {
            Name                  = "Basic Straw Mix",
            Description           = "Simple wheat straw substrate suitable for oyster mushrooms.",
            MoistureContentTarget = 65m,
            FinalMixtureSizeKg    = 10m,
            ProjectId             = project.Id
        };
        recipe.Ingredients.Add(new RecipeIngredient { IngredientId = ws.Id,  Amount = 80m });
        recipe.Ingredients.Add(new RecipeIngredient { IngredientId = wb.Id,  Amount = 15m });
        recipe.Ingredients.Add(new RecipeIngredient { IngredientId = oat.Id, Amount = 5m  });
        db.SubstrateRecipes.Add(recipe);

        await db.SaveChangesAsync();
    }
}
