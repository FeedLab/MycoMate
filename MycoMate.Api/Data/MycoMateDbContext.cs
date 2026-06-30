using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data;

public class MycoMateDbContext(DbContextOptions<MycoMateDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Mineral> Minerals => Set<Mineral>();
    public DbSet<Vitamin> Vitamins => Set<Vitamin>();
    public DbSet<AminoAcid> AminoAcids => Set<AminoAcid>();
    public DbSet<IngredientMineral> IngredientMinerals => Set<IngredientMineral>();
    public DbSet<IngredientVitamin> IngredientVitamins => Set<IngredientVitamin>();
    public DbSet<IngredientAminoAcid> IngredientAminoAcids => Set<IngredientAminoAcid>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();
    public DbSet<SubstrateRecipe> SubstrateRecipes => Set<SubstrateRecipe>();
    public DbSet<RecipeIngredient> RecipeIngredients => Set<RecipeIngredient>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(MycoMateDbContext).Assembly);
    }
}
