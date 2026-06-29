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
            new Ingredient { ShortName = "WS",  DisplayName = "Wheat Straw",      MoistureContent = 10.0m, UserId = "system" },
            new Ingredient { ShortName = "HB",  DisplayName = "Hardwood Bran",    MoistureContent = 12.0m, UserId = "system" },
            new Ingredient { ShortName = "WB",  DisplayName = "Wheat Bran",       MoistureContent = 11.5m, UserId = "system" },
            new Ingredient { ShortName = "RB",  DisplayName = "Rice Bran",        MoistureContent =  9.5m, UserId = "system" },
            new Ingredient { ShortName = "OAT", DisplayName = "Oat Bran",         MoistureContent = 10.5m, UserId = "system" },
            new Ingredient { ShortName = "H2O", DisplayName = "Water",            MoistureContent = 100m,  UserId = "system" },
            new Ingredient { ShortName = "SD",  DisplayName = "Sawdust",          MoistureContent = 22.0m, UserId = "system" },
            new Ingredient { ShortName = "BRB", DisplayName = "Brown Rice Bran",  MoistureContent = 12.0m, UserId = "system" },
            new Ingredient { ShortName = "DOL", DisplayName = "Dolomite",         MoistureContent =  0.0m, UserId = "system" },
            new Ingredient { ShortName = "GYP", DisplayName = "Gypsum",           MoistureContent =  0.0m, UserId = "system" },
            new Ingredient { ShortName = "SBM", DisplayName = "Soybean Meal 48%", MoistureContent = 11.0m, UserId = "system" },
            new Ingredient { ShortName = "PUM", DisplayName = "Pumice",           MoistureContent =  3.0m, UserId = "system" },
            new Ingredient { ShortName = "MOL", DisplayName = "Molasses",         MoistureContent = 10.0m, UserId = "system" },
            new Ingredient { ShortName = "RH",  DisplayName = "Rice Husk",        MoistureContent = 12.0m, UserId = "system" }
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

        var projectAdmin = new Project
        {
            Name        = "Sample Project Owner",
            OwnerId     = admin.Id,
            Description = "Owned by admin@mycomate.com. Demonstrates full owner access: create and manage recipes, invite members, and configure project settings. user@mycomate.com has editor access and reader@mycomate.com has read-only access."
        };
        projectAdmin.Members.Add(new ProjectMember { UserId = admin.Id,  Role = ProjectRole.Owner    });
        projectAdmin.Members.Add(new ProjectMember { UserId = editor.Id, Role = ProjectRole.Editor   });
        projectAdmin.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.ReadOnly });
        db.Projects.Add(projectAdmin);

        var projectReader = new Project
        {
            Name        = "Sample Project ReadOnly",
            OwnerId     = reader.Id,
            Description = "Owned by reader@mycomate.com. Demonstrates a project where the owner has full control but collaborators are restricted to read-only access. admin@mycomate.com can view all recipes and ingredients but cannot make changes."
        };
        projectReader.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.Owner    });
        projectReader.Members.Add(new ProjectMember { UserId = admin.Id,  Role = ProjectRole.ReadOnly });
        db.Projects.Add(projectReader);

        var projectEditor = new Project
        {
            Name        = "Sample Project Editor",
            OwnerId     = editor.Id,
            Description = "Owned by user@mycomate.com. Demonstrates collaborative editing: admin@mycomate.com can create and modify substrate recipes and ingredients, while reader@mycomate.com can browse all content without making changes."
        };
        projectEditor.Members.Add(new ProjectMember { UserId = editor.Id, Role = ProjectRole.Owner    });
        projectEditor.Members.Add(new ProjectMember { UserId = admin.Id,  Role = ProjectRole.Editor   });
        projectEditor.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.ReadOnly });
        db.Projects.Add(projectEditor);

        var ws  = await db.Ingredients.FirstAsync(i => i.ShortName == "WS");
        var wb  = await db.Ingredients.FirstAsync(i => i.ShortName == "WB");
        var oat = await db.Ingredients.FirstAsync(i => i.ShortName == "OAT");
        var sd  = await db.Ingredients.FirstAsync(i => i.ShortName == "SD");
        var brb = await db.Ingredients.FirstAsync(i => i.ShortName == "BRB");
        var dol = await db.Ingredients.FirstAsync(i => i.ShortName == "DOL");
        var gyp = await db.Ingredients.FirstAsync(i => i.ShortName == "GYP");
        var sbm = await db.Ingredients.FirstAsync(i => i.ShortName == "SBM");
        var pum = await db.Ingredients.FirstAsync(i => i.ShortName == "PUM");
        var mol = await db.Ingredients.FirstAsync(i => i.ShortName == "MOL");
        var rh  = await db.Ingredients.FirstAsync(i => i.ShortName == "RH");

        // CurrentWater = 10 - 8.976 = 1.024 kg, TargetWater = 10 × 0.65 = 6.5 kg → WaterAdjustment = 5.476 kg = 54.76%
        var recipe = new SubstrateRecipe
        {
            Name                    = "Basic Straw Mix",
            Description             = "Simple wheat straw substrate suitable for oyster mushrooms.",
            MoistureContentTarget   = 65m,
            FinalMixtureSizeKg      = 10m,
            WaterAdjustmentPercent  = 54.76m,
            ProjectId               = projectAdmin.Id
        };
        // WetMatter = FinalMixtureSizeKg × (WetAmountPercent/100), DryMatter = WetMatter × (1 - MC/100)
        // WS:  WetMatter=8.000, DryMatter=7.200 | WB: WetMatter=1.500, DryMatter=1.328 | OAT: WetMatter=0.500, DryMatter=0.448
        // TotalDry=8.976, DryAmountPercent: WS=80.2139, WB=14.7955, OAT=4.9906
        recipe.Ingredients.Add(new RecipeIngredient { IngredientId = ws.Id,  WetAmount = 80m, WetAmountPercent = 80m, WetMatter = 8.000m, MoistureContent = ws.MoistureContent,  DryMatter = 7.200m, DryAmountPercent = 80.2139m });
        recipe.Ingredients.Add(new RecipeIngredient { IngredientId = wb.Id,  WetAmount = 15m, WetAmountPercent = 15m, WetMatter = 1.500m, MoistureContent = wb.MoistureContent,  DryMatter = 1.328m, DryAmountPercent = 14.7955m });
        recipe.Ingredients.Add(new RecipeIngredient { IngredientId = oat.Id, WetAmount = 5m,  WetAmountPercent = 5m,  WetMatter = 0.500m, MoistureContent = oat.MoistureContent, DryMatter = 0.448m, DryAmountPercent = 4.9906m  });
        db.SubstrateRecipes.Add(recipe);

        // Wanphen Farm — Nang Faa variety, MC target 60%, 379 kg batch
        // WetAmount totals 189.68 kg, scaled to FinalMixtureSizeKg=379 via WetAmountPercent
        // CurrentWater=76.005 kg, TargetWater=227.4 kg → WaterAdjustment=151.395 kg = 39.9459%
        var recipeWanphen = new SubstrateRecipe
        {
            Name                   = "Wanphen Farm",
            Description            = "Nang Faa",
            MoistureContentTarget  = 60m,
            FinalMixtureSizeKg     = 379m,
            WaterAdjustmentPercent = 39.9459m,
            ProjectId              = projectAdmin.Id
        };
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = sd.Id,  WetAmount = 160.00m, WetAmountPercent = 84.3527m, WetMatter = 319.697m, MoistureContent = sd.MoistureContent,  DryMatter = 249.364m, DryAmountPercent = 82.2993m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = brb.Id, WetAmount =  11.35m, WetAmountPercent =  5.9837m, WetMatter =  22.678m, MoistureContent = brb.MoistureContent, DryMatter =  19.957m, DryAmountPercent =  6.5864m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = dol.Id, WetAmount =   1.25m, WetAmountPercent =  0.6590m, WetMatter =   2.498m, MoistureContent = dol.MoistureContent, DryMatter =   2.498m, DryAmountPercent =  0.8244m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = gyp.Id, WetAmount =   1.87m, WetAmountPercent =  0.9859m, WetMatter =   3.737m, MoistureContent = gyp.MoistureContent, DryMatter =   3.737m, DryAmountPercent =  1.2331m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = sbm.Id, WetAmount =   0.00m, WetAmountPercent =  0.0000m, WetMatter =   0.000m, MoistureContent = sbm.MoistureContent, DryMatter =   0.000m, DryAmountPercent =  0.0000m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = pum.Id, WetAmount =   3.86m, WetAmountPercent =  2.0350m, WetMatter =   7.713m, MoistureContent = pum.MoistureContent, DryMatter =   7.482m, DryAmountPercent =  2.4694m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = mol.Id, WetAmount =   0.00m, WetAmountPercent =  0.0000m, WetMatter =   0.000m, MoistureContent = mol.MoistureContent, DryMatter =   0.000m, DryAmountPercent =  0.0000m });
        recipeWanphen.Ingredients.Add(new RecipeIngredient { IngredientId = rh.Id,  WetAmount =  11.35m, WetAmountPercent =  5.9837m, WetMatter =  22.678m, MoistureContent = rh.MoistureContent,  DryMatter =  19.957m, DryAmountPercent =  6.5864m });
        db.SubstrateRecipes.Add(recipeWanphen);

        await db.SaveChangesAsync();
    }
}
