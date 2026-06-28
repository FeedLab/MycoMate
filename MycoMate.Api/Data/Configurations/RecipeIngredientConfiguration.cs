using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class RecipeIngredientConfiguration : IEntityTypeConfiguration<RecipeIngredient>
{
    public void Configure(EntityTypeBuilder<RecipeIngredient> builder)
    {
        builder.HasKey(ri => new { ri.RecipeId, ri.IngredientId });

        builder.Property(ri => ri.WetAmount)
            .HasPrecision(10, 3);

        builder.Property(ri => ri.WetAmountPercent)
            .HasPrecision(7, 4);

        builder.Property(ri => ri.MoistureContent)
            .HasPrecision(5, 2);

        builder.HasOne(ri => ri.Recipe)
            .WithMany(r => r.Ingredients)
            .HasForeignKey(ri => ri.RecipeId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ri => ri.Ingredient)
            .WithMany()
            .HasForeignKey(ri => ri.IngredientId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
