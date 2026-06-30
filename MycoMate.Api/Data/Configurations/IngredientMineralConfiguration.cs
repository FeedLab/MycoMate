using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class IngredientMineralConfiguration : IEntityTypeConfiguration<IngredientMineral>
{
    public void Configure(EntityTypeBuilder<IngredientMineral> builder)
    {
        builder.HasKey(im => new { im.IngredientId, im.MineralId });

        builder.Property(im => im.Value).HasPrecision(10, 4);

        builder.HasOne(im => im.Ingredient)
            .WithMany(i => i.Minerals)
            .HasForeignKey(im => im.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(im => im.Mineral)
            .WithMany()
            .HasForeignKey(im => im.MineralId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
