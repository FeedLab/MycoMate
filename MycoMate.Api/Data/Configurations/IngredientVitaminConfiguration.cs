using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class IngredientVitaminConfiguration : IEntityTypeConfiguration<IngredientVitamin>
{
    public void Configure(EntityTypeBuilder<IngredientVitamin> builder)
    {
        builder.HasKey(iv => new { iv.IngredientId, iv.VitaminId });

        builder.Property(iv => iv.Value).HasPrecision(10, 4);

        builder.HasOne(iv => iv.Ingredient)
            .WithMany(i => i.Vitamins)
            .HasForeignKey(iv => iv.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(iv => iv.Vitamin)
            .WithMany()
            .HasForeignKey(iv => iv.VitaminId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
