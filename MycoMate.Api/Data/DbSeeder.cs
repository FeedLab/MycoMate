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
            new { Email = "admin@mycomate.com", Password = "Admin123$" },
            new { Email = "user@mycomate.com", Password = "User123$" },
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

        // ── 1. Seed lookup tables ─────────────────────────────────────────────

        var minerals = new List<Mineral>
        {
            new() { Name = "Calcium", ShortName = "Ca", Description = "Structural cell wall component", Unit = "%" },
            new()
            {
                Name = "Potassium", ShortName = "K", Description = "Enzyme activation and osmoregulation", Unit = "%"
            },
            new() { Name = "Magnesium", ShortName = "Mg", Description = "Chlorophyll and enzyme cofactor", Unit = "%" },
            new()
            {
                Name = "Phosphorus", ShortName = "P", Description = "Energy transfer and nucleic acid synthesis",
                Unit = "%"
            },
            new()
            {
                Name = "Silicon", ShortName = "Si", Description = "Cell wall rigidity and stress resistance", Unit = "%"
            },
            new()
            {
                Name = "Iron", ShortName = "Fe", Description = "Electron transport and enzyme cofactor", Unit = "%"
            },
            new() { Name = "Zinc", ShortName = "Zn", Description = "Enzyme structure and immune function", Unit = "%" },
            new() { Name = "Manganese", ShortName = "Mn", Description = "Antioxidant enzyme cofactor", Unit = "%" },
            new()
            {
                Name = "Sulfur", ShortName = "S", Description = "Amino acid synthesis and enzyme cofactor", Unit = "%"
            },
            new() { Name = "Aluminum", ShortName = "Al", Description = "Secondary structural oxide", Unit = "%" },
        };
        db.Minerals.AddRange(minerals);

        var vitamins = new List<Vitamin>
        {
            new()
            {
                Name = "Thiamine", ShortName = "B1", Description = "Carbohydrate metabolism coenzyme", Unit = "mg/kg DM"
            },
            new()
            {
                Name = "Riboflavin", ShortName = "B2", Description = "Electron carrier in redox reactions",
                Unit = "mg/kg DM"
            },
            new()
            {
                Name = "Niacin", ShortName = "B3", Description = "NAD/NADP coenzyme for energy metabolism",
                Unit = "mg/kg DM"
            },
            new()
            {
                Name = "Pantothenic Acid", ShortName = "B5", Description = "CoA synthesis and fatty acid metabolism",
                Unit = "mg/kg DM"
            },
            new()
            {
                Name = "Pyridoxine", ShortName = "B6", Description = "Amino acid metabolism coenzyme", Unit = "mg/kg DM"
            },
            new()
            {
                Name = "Folate", ShortName = "B9", Description = "One-carbon transfer in nucleotide synthesis",
                Unit = "mg/kg DM"
            },
            new()
            {
                Name = "Tocopherol", ShortName = "E", Description = "Lipid-soluble antioxidant", Unit = "mg/kg DM"
            },
        };
        db.Vitamins.AddRange(vitamins);

        var aminoAcids = new List<AminoAcid>
        {
            new()
            {
                Name = "Glycine", ShortName = "Gly", Description = "Simplest amino acid, cell wall precursor",
                Unit = "%"
            },
            new()
            {
                Name = "Alanine", ShortName = "Ala", Description = "Energy metabolism and gluconeogenesis", Unit = "%"
            },
            new()
            {
                Name = "Glutamic Acid", ShortName = "Glu", Description = "Central nitrogen metabolism hub", Unit = "%"
            },
            new()
            {
                Name = "Leucine", ShortName = "Leu", Description = "Branched-chain essential amino acid", Unit = "%"
            },
            new()
            {
                Name = "Aspartic Acid", ShortName = "Asp", Description = "Nitrogen transport and urea cycle", Unit = "%"
            },
            new()
            {
                Name = "Proline", ShortName = "Pro", Description = "Collagen structure and stress response", Unit = "%"
            },
            new()
            {
                Name = "Arginine", ShortName = "Arg", Description = "Nitrogen storage and immune function", Unit = "%"
            },
            new()
            {
                Name = "Lysine", ShortName = "Lys", Description = "Essential amino acid for protein synthesis",
                Unit = "%"
            },
            new()
            {
                Name = "Methionine", ShortName = "Met", Description = "Sulfur-containing essential amino acid",
                Unit = "%"
            },
            new()
            {
                Name = "Tryptophan", ShortName = "Trp", Description = "Precursor to serotonin and niacin", Unit = "%"
            },
            new()
            {
                Name = "Threonine", ShortName = "Thr", Description = "Essential amino acid for immune function",
                Unit = "%"
            },
        };
        db.AminoAcids.AddRange(aminoAcids);

        await db.SaveChangesAsync();

        // ── 2. Build lookup dictionaries by short name ────────────────────────

        var m = minerals.ToDictionary(x => x.ShortName);
        var v = vitamins.ToDictionary(x => x.ShortName);
        var aa = aminoAcids.ToDictionary(x => x.ShortName);

        // ── 3. Seed ingredients with join-table references ────────────────────

        db.Ingredients.AddRange(
            new Ingredient
            {
                ShortName = "WS", DisplayName = "Wheat Straw", MoistureContent = 10.0m, UserId = "system",
                CarbonToNitrogenRatio = 80m, PhLevel = 6.5m,
                Function = "Bulk substrate, primary carbon source for oyster and straw-loving species",
                BulkDensityKgPerM3 = 40m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.30m },
                    new() { MineralId = m["K"].Id, Value = 1.20m },
                    new() { MineralId = m["Mg"].Id, Value = 0.10m },
                    new() { MineralId = m["P"].Id, Value = 0.08m },
                    new() { MineralId = m["Si"].Id, Value = 3.50m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 0.4m },
                    new() { VitaminId = v["B2"].Id, Value = 0.6m },
                    new() { VitaminId = v["B3"].Id, Value = 8.5m },
                ],
                AminoAcids =
                [
                    new() { AminoAcidId = aa["Gly"].Id, Value = 3.2m },
                    new() { AminoAcidId = aa["Ala"].Id, Value = 4.1m },
                    new() { AminoAcidId = aa["Glu"].Id, Value = 13.5m },
                    new() { AminoAcidId = aa["Leu"].Id, Value = 5.8m },
                ]
            },
            new Ingredient
            {
                ShortName = "HB", DisplayName = "Hardwood Bran", MoistureContent = 12.0m, UserId = "system",
                CarbonToNitrogenRatio = 120m, PhLevel = 6.0m,
                Function = "Carbon source and bulk substrate for wood-loving species",
                BulkDensityKgPerM3 = 180m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.15m },
                    new() { MineralId = m["K"].Id, Value = 0.60m },
                    new() { MineralId = m["Mg"].Id, Value = 0.08m },
                    new() { MineralId = m["P"].Id, Value = 0.05m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 0.2m },
                    new() { VitaminId = v["B3"].Id, Value = 4.0m },
                ]
            },
            new Ingredient
            {
                ShortName = "WB", DisplayName = "Wheat Bran", MoistureContent = 11.5m, UserId = "system",
                CarbonToNitrogenRatio = 15m, PhLevel = 6.2m,
                Function = "Nitrogen supplement and energy source, accelerates colonisation",
                BulkDensityKgPerM3 = 310m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.11m },
                    new() { MineralId = m["K"].Id, Value = 1.19m },
                    new() { MineralId = m["Mg"].Id, Value = 0.40m },
                    new() { MineralId = m["P"].Id, Value = 1.05m },
                    new() { MineralId = m["Fe"].Id, Value = 0.006m },
                    new() { MineralId = m["Zn"].Id, Value = 0.007m },
                    new() { MineralId = m["Mn"].Id, Value = 0.011m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 7.9m },
                    new() { VitaminId = v["B2"].Id, Value = 0.9m },
                    new() { VitaminId = v["B3"].Id, Value = 55.0m },
                    new() { VitaminId = v["B6"].Id, Value = 1.3m },
                    new() { VitaminId = v["B9"].Id, Value = 0.6m },
                    new() { VitaminId = v["E"].Id, Value = 14.0m },
                ],
                AminoAcids =
                [
                    new() { AminoAcidId = aa["Glu"].Id, Value = 28.0m },
                    new() { AminoAcidId = aa["Pro"].Id, Value = 9.2m },
                    new() { AminoAcidId = aa["Leu"].Id, Value = 6.8m },
                    new() { AminoAcidId = aa["Arg"].Id, Value = 5.3m },
                    new() { AminoAcidId = aa["Lys"].Id, Value = 3.6m },
                ]
            },
            new Ingredient
            {
                ShortName = "RB", DisplayName = "Rice Bran", MoistureContent = 9.5m, UserId = "system",
                CarbonToNitrogenRatio = 20m, PhLevel = 6.0m,
                Function = "Nitrogen and lipid supplement, rich in B vitamins",
                BulkDensityKgPerM3 = 350m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.06m },
                    new() { MineralId = m["K"].Id, Value = 1.60m },
                    new() { MineralId = m["Mg"].Id, Value = 0.80m },
                    new() { MineralId = m["P"].Id, Value = 1.80m },
                    new() { MineralId = m["Fe"].Id, Value = 0.007m },
                    new() { MineralId = m["Zn"].Id, Value = 0.005m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 22.0m },
                    new() { VitaminId = v["B2"].Id, Value = 0.3m },
                    new() { VitaminId = v["B3"].Id, Value = 299.0m },
                    new() { VitaminId = v["B5"].Id, Value = 7.1m },
                    new() { VitaminId = v["B6"].Id, Value = 4.1m },
                    new() { VitaminId = v["E"].Id, Value = 32.0m },
                ],
                AminoAcids =
                [
                    new() { AminoAcidId = aa["Glu"].Id, Value = 14.0m },
                    new() { AminoAcidId = aa["Asp"].Id, Value = 9.5m },
                    new() { AminoAcidId = aa["Leu"].Id, Value = 7.9m },
                    new() { AminoAcidId = aa["Lys"].Id, Value = 4.1m },
                    new() { AminoAcidId = aa["Met"].Id, Value = 2.2m },
                ]
            },
            new Ingredient
            {
                ShortName = "OAT", DisplayName = "Oat Bran", MoistureContent = 10.5m, UserId = "system",
                CarbonToNitrogenRatio = 18m, PhLevel = 6.5m,
                Function = "Nitrogen supplement with beta-glucans, supports mycelium vigour",
                BulkDensityKgPerM3 = 280m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.05m },
                    new() { MineralId = m["K"].Id, Value = 0.43m },
                    new() { MineralId = m["Mg"].Id, Value = 0.18m },
                    new() { MineralId = m["P"].Id, Value = 0.71m },
                    new() { MineralId = m["Fe"].Id, Value = 0.004m },
                    new() { MineralId = m["Zn"].Id, Value = 0.004m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 6.6m },
                    new() { VitaminId = v["B2"].Id, Value = 0.2m },
                    new() { VitaminId = v["B3"].Id, Value = 9.4m },
                    new() { VitaminId = v["B5"].Id, Value = 1.4m },
                    new() { VitaminId = v["B6"].Id, Value = 0.2m },
                ],
                AminoAcids =
                [
                    new() { AminoAcidId = aa["Glu"].Id, Value = 22.0m },
                    new() { AminoAcidId = aa["Ala"].Id, Value = 5.0m },
                    new() { AminoAcidId = aa["Leu"].Id, Value = 7.5m },
                    new() { AminoAcidId = aa["Lys"].Id, Value = 4.6m },
                    new() { AminoAcidId = aa["Thr"].Id, Value = 3.7m },
                ]
            },
            new Ingredient
            {
                ShortName = "SD", DisplayName = "Sawdust", MoistureContent = 22.0m, UserId = "system",
                CarbonToNitrogenRatio = 400m, PhLevel = 5.5m,
                Function = "Primary bulk substrate and carbon source for wood-decomposing species",
                BulkDensityKgPerM3 = 165m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.12m },
                    new() { MineralId = m["K"].Id, Value = 0.08m },
                    new() { MineralId = m["Mg"].Id, Value = 0.03m },
                    new() { MineralId = m["P"].Id, Value = 0.02m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 0.1m },
                ]
            },
            new Ingredient
            {
                ShortName = "BRB", DisplayName = "Brown Rice Bran", MoistureContent = 12.0m, UserId = "system",
                CarbonToNitrogenRatio = 22m, PhLevel = 6.2m,
                Function = "Nitrogen and energy supplement, less processed than white rice bran",
                BulkDensityKgPerM3 = 320m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.07m },
                    new() { MineralId = m["K"].Id, Value = 1.40m },
                    new() { MineralId = m["Mg"].Id, Value = 0.75m },
                    new() { MineralId = m["P"].Id, Value = 1.65m },
                    new() { MineralId = m["Fe"].Id, Value = 0.006m },
                    new() { MineralId = m["Zn"].Id, Value = 0.005m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 18.0m },
                    new() { VitaminId = v["B2"].Id, Value = 0.3m },
                    new() { VitaminId = v["B3"].Id, Value = 280.0m },
                    new() { VitaminId = v["B5"].Id, Value = 6.8m },
                    new() { VitaminId = v["B6"].Id, Value = 3.8m },
                    new() { VitaminId = v["E"].Id, Value = 28.0m },
                ],
                AminoAcids =
                [
                    new() { AminoAcidId = aa["Glu"].Id, Value = 13.5m },
                    new() { AminoAcidId = aa["Asp"].Id, Value = 9.0m },
                    new() { AminoAcidId = aa["Leu"].Id, Value = 7.5m },
                    new() { AminoAcidId = aa["Lys"].Id, Value = 3.9m },
                ]
            },
            new Ingredient
            {
                ShortName = "DOL", DisplayName = "Dolomite", MoistureContent = 0.0m, UserId = "system",
                PhLevel = 9.5m,
                Function = "pH buffer, provides calcium and magnesium, corrects acidic substrates",
                BulkDensityKgPerM3 = 900m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 21.50m },
                    new() { MineralId = m["Mg"].Id, Value = 13.00m },
                ]
            },
            new Ingredient
            {
                ShortName = "GYP", DisplayName = "Gypsum", MoistureContent = 0.0m, UserId = "system",
                PhLevel = 7.0m,
                Function = "pH buffer, calcium and sulfur source, improves substrate structure",
                BulkDensityKgPerM3 = 1100m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 23.20m },
                    new() { MineralId = m["S"].Id, Value = 18.60m },
                ]
            },
            new Ingredient
            {
                ShortName = "SBM", DisplayName = "Soybean Meal", MoistureContent = 11.0m, UserId = "system",
                CarbonToNitrogenRatio = 4m, PhLevel = 6.5m,
                Function = "High-protein nitrogen supplement, promotes rapid colonisation",
                BulkDensityKgPerM3 = 600m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.30m },
                    new() { MineralId = m["K"].Id, Value = 2.00m },
                    new() { MineralId = m["Mg"].Id, Value = 0.30m },
                    new() { MineralId = m["P"].Id, Value = 0.70m },
                    new() { MineralId = m["Fe"].Id, Value = 0.01m },
                    new() { MineralId = m["Zn"].Id, Value = 0.005m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 3.7m },
                    new() { VitaminId = v["B2"].Id, Value = 3.0m },
                    new() { VitaminId = v["B3"].Id, Value = 29.0m },
                    new() { VitaminId = v["B9"].Id, Value = 1.4m },
                ],
                AminoAcids =
                [
                    new() { AminoAcidId = aa["Glu"].Id, Value = 18.5m },
                    new() { AminoAcidId = aa["Asp"].Id, Value = 11.7m },
                    new() { AminoAcidId = aa["Leu"].Id, Value = 7.8m },
                    new() { AminoAcidId = aa["Lys"].Id, Value = 6.3m },
                    new() { AminoAcidId = aa["Arg"].Id, Value = 7.4m },
                    new() { AminoAcidId = aa["Met"].Id, Value = 1.4m },
                    new() { AminoAcidId = aa["Trp"].Id, Value = 1.4m },
                ]
            },
            new Ingredient
            {
                ShortName = "PUM", DisplayName = "Pumice", MoistureContent = 3.0m, UserId = "system",
                PhLevel = 7.2m,
                Function = "Aeration agent, improves drainage and gas exchange in dense substrates",
                BulkDensityKgPerM3 = 700m,
                Minerals =
                [
                    new() { MineralId = m["Si"].Id, Value = 70.00m },
                    new() { MineralId = m["Al"].Id, Value = 12.00m },
                    new() { MineralId = m["Ca"].Id, Value = 2.00m },
                    new() { MineralId = m["Mg"].Id, Value = 0.50m },
                ]
            },
            new Ingredient
            {
                ShortName = "MOL", DisplayName = "Molasses", MoistureContent = 10.0m, UserId = "system",
                CarbonToNitrogenRatio = 50m, PhLevel = 5.8m,
                Function = "Readily available carbon and energy source, stimulates microbial activity",
                BulkDensityKgPerM3 = 1400m,
                Minerals =
                [
                    new() { MineralId = m["Ca"].Id, Value = 0.80m },
                    new() { MineralId = m["K"].Id, Value = 3.60m },
                    new() { MineralId = m["Mg"].Id, Value = 0.50m },
                    new() { MineralId = m["Fe"].Id, Value = 0.01m },
                ],
                Vitamins =
                [
                    new() { VitaminId = v["B1"].Id, Value = 0.2m },
                    new() { VitaminId = v["B2"].Id, Value = 0.2m },
                    new() { VitaminId = v["B3"].Id, Value = 2.0m },
                    new() { VitaminId = v["B6"].Id, Value = 0.7m },
                    new() { VitaminId = v["B9"].Id, Value = 0.1m },
                ]
            },
            new Ingredient
            {
                ShortName = "RH", DisplayName = "Rice Husk", MoistureContent = 12.0m, UserId = "system",
                CarbonToNitrogenRatio = 120m, PhLevel = 6.5m,
                Function = "Aeration agent and carbon source, highly resistant to decomposition",
                BulkDensityKgPerM3 = 110m,
                Minerals =
                [
                    new() { MineralId = m["Si"].Id, Value = 15.00m },
                    new() { MineralId = m["Ca"].Id, Value = 0.06m },
                    new() { MineralId = m["K"].Id, Value = 0.30m },
                    new() { MineralId = m["Mg"].Id, Value = 0.05m },
                    new() { MineralId = m["P"].Id, Value = 0.05m },
                ]
            }
        );

        await db.SaveChangesAsync();
    }

    private static async Task SeedProjectAsync(MycoMateDbContext db, UserManager<IdentityUser> userManager)
    {
        if (await db.Projects.AnyAsync())
            return;

        var admin = await userManager.FindByEmailAsync("admin@mycomate.com");
        var editor = await userManager.FindByEmailAsync("user@mycomate.com");
        var reader = await userManager.FindByEmailAsync("reader@mycomate.com");

        if (admin is null || editor is null || reader is null)
            return;

        var projectAdmin = new Project
        {
            Name = "Sample Project Owner",
            OwnerId = admin.Id,
            Description =
                "Owned by admin@mycomate.com. Demonstrates full owner access: create and manage recipes, invite members, and configure project settings. user@mycomate.com has editor access and reader@mycomate.com has read-only access."
        };
        projectAdmin.Members.Add(new ProjectMember { UserId = admin.Id, Role = ProjectRole.Owner });
        projectAdmin.Members.Add(new ProjectMember { UserId = editor.Id, Role = ProjectRole.Editor });
        projectAdmin.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.ReadOnly });
        db.Projects.Add(projectAdmin);

        var projectReader = new Project
        {
            Name = "Sample Project ReadOnly",
            OwnerId = reader.Id,
            Description =
                "Owned by reader@mycomate.com. Demonstrates a project where the owner has full control but collaborators are restricted to read-only access. admin@mycomate.com can view all recipes and ingredients but cannot make changes."
        };
        projectReader.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.Owner });
        projectReader.Members.Add(new ProjectMember { UserId = admin.Id, Role = ProjectRole.ReadOnly });
        db.Projects.Add(projectReader);

        var projectEditor = new Project
        {
            Name = "Sample Project Editor",
            OwnerId = editor.Id,
            Description =
                "Owned by user@mycomate.com. Demonstrates collaborative editing: admin@mycomate.com can create and modify substrate recipes and ingredients, while reader@mycomate.com can browse all content without making changes."
        };
        projectEditor.Members.Add(new ProjectMember { UserId = editor.Id, Role = ProjectRole.Owner });
        projectEditor.Members.Add(new ProjectMember { UserId = admin.Id, Role = ProjectRole.Editor });
        projectEditor.Members.Add(new ProjectMember { UserId = reader.Id, Role = ProjectRole.ReadOnly });
        db.Projects.Add(projectEditor);

        var ws = await db.Ingredients.FirstAsync(i => i.ShortName == "WS");
        var wb = await db.Ingredients.FirstAsync(i => i.ShortName == "WB");
        var oat = await db.Ingredients.FirstAsync(i => i.ShortName == "OAT");
        var sd = await db.Ingredients.FirstAsync(i => i.ShortName == "SD");
        var brb = await db.Ingredients.FirstAsync(i => i.ShortName == "BRB");
        var dol = await db.Ingredients.FirstAsync(i => i.ShortName == "DOL");
        var gyp = await db.Ingredients.FirstAsync(i => i.ShortName == "GYP");
        var sbm = await db.Ingredients.FirstAsync(i => i.ShortName == "SBM");
        var pum = await db.Ingredients.FirstAsync(i => i.ShortName == "PUM");
        var mol = await db.Ingredients.FirstAsync(i => i.ShortName == "MOL");
        var rh = await db.Ingredients.FirstAsync(i => i.ShortName == "RH");

        // DryPercent = % of total dry matter. TotalDryKg = 10 × 0.35 = 3.5 kg
        // WaterFromIngredients ≈ 0.400 kg, TargetWater = 6.5 kg → WaterAdjustment = 61.0%
        var recipe = new SubstrateRecipe
        {
            Name = "Basic Straw Mix",
            Description = "Simple wheat straw substrate suitable for oyster mushrooms.",
            MoistureContentTarget = 65m,
            FinalMixtureSizeKg = 10m,
            WaterAdjustmentPercent = 61.0m,
            ProjectId = projectAdmin.Id
        };
        recipe.Ingredients.Add(new RecipeIngredient
            { IngredientId = ws.Id, DryPercent = 80.2139m, MoistureContent = ws.MoistureContent });
        recipe.Ingredients.Add(new RecipeIngredient
            { IngredientId = wb.Id, DryPercent = 14.7955m, MoistureContent = wb.MoistureContent });
        recipe.Ingredients.Add(new RecipeIngredient
            { IngredientId = oat.Id, DryPercent = 4.9906m, MoistureContent = oat.MoistureContent });
        db.SubstrateRecipes.Add(recipe);

        // DryPercent = % of total dry matter. TotalDryKg = 379 × 0.40 = 151.6 kg
        // WaterFromIngredients ≈ 38.030 kg, TargetWater = 227.4 kg → WaterAdjustment = 49.965%
        var recipeWanphenHigh = new SubstrateRecipe
        {
            Name = "Wanphen Farm (High Risk)",
            Description = "Bhutan (Hed Nang Faa Phu-Than): Due to high nutrient availability, this mixture is highly susceptible to rapid mold growth and bacterial contamination if not processed correctly",
            MoistureContentTarget = 60m,
            FinalMixtureSizeKg = 379m,
            WaterAdjustmentPercent = 49.965m,
            ProjectId = projectAdmin.Id
        };
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = sd.Id, DryPercent = 82.2993m, MoistureContent = sd.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = brb.Id, DryPercent = 15.0m, MoistureContent = brb.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = dol.Id, DryPercent = 0.8244m, MoistureContent = dol.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = gyp.Id, DryPercent = 1.2331m, MoistureContent = gyp.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = sbm.Id, DryPercent = 0.0000m, MoistureContent = sbm.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = pum.Id, DryPercent = 2.4694m, MoistureContent = pum.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = mol.Id, DryPercent = 0.0000m, MoistureContent = mol.MoistureContent });
        recipeWanphenHigh.Ingredients.Add(new RecipeIngredient
            { IngredientId = rh.Id, DryPercent = 6.5864m, MoistureContent = rh.MoistureContent });
        db.SubstrateRecipes.Add(recipeWanphenHigh);


        var recipeWanphenMedium = new SubstrateRecipe
        {
            Name = "Wanphen Farm (Medium Risk)",
            Description = "Bhutan (Hed Nang Faa Phu-Than): This organic brown rice substrate is a high-nutrient medium that can easily harbor mold or bacteria",
            MoistureContentTarget = 60m,
            FinalMixtureSizeKg = 379m,
            WaterAdjustmentPercent = 49.965m,
            ProjectId = projectAdmin.Id
        };
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = sd.Id, DryPercent = 82.2993m, MoistureContent = sd.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = brb.Id, DryPercent = 6.5864m, MoistureContent = brb.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = dol.Id, DryPercent = 0.8244m, MoistureContent = dol.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = gyp.Id, DryPercent = 1.2331m, MoistureContent = gyp.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = sbm.Id, DryPercent = 0.0000m, MoistureContent = sbm.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = pum.Id, DryPercent = 2.4694m, MoistureContent = pum.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = mol.Id, DryPercent = 0.0000m, MoistureContent = mol.MoistureContent });
        recipeWanphenMedium.Ingredients.Add(new RecipeIngredient
            { IngredientId = rh.Id, DryPercent = 6.5864m, MoistureContent = rh.MoistureContent });
        db.SubstrateRecipes.Add(recipeWanphenMedium);

        var recipeWanphenLow = new SubstrateRecipe
        {
            Name = "Wanphen Farm (Low Risk)",
            Description = "Bhutan (Hed Nang Faa Phu-Than): ",
            MoistureContentTarget = 60m,
            FinalMixtureSizeKg = 379m,
            WaterAdjustmentPercent = 49.965m,
            ProjectId = projectAdmin.Id
        };
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = sd.Id, DryPercent = 82.2993m, MoistureContent = sd.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = brb.Id, DryPercent = 6.0m, MoistureContent = brb.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = dol.Id, DryPercent = 0.8244m, MoistureContent = dol.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = gyp.Id, DryPercent = 1.2331m, MoistureContent = gyp.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = sbm.Id, DryPercent = 0.0000m, MoistureContent = sbm.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = pum.Id, DryPercent = 2.4694m, MoistureContent = pum.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = mol.Id, DryPercent = 0.0000m, MoistureContent = mol.MoistureContent });
        recipeWanphenLow.Ingredients.Add(new RecipeIngredient
            { IngredientId = rh.Id, DryPercent = 6.5864m, MoistureContent = rh.MoistureContent });
        db.SubstrateRecipes.Add(recipeWanphenLow);
        db.SaveChangesAsync();
    }
}