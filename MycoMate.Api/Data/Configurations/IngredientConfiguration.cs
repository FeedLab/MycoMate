using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class IngredientConfiguration : IEntityTypeConfiguration<Ingredient>
{
    public void Configure(EntityTypeBuilder<Ingredient> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.MoistureContent).HasPrecision(5, 2);
        builder.Property(i => i.CarbonToNitrogenRatio).HasPrecision(7, 2);
        builder.Property(i => i.PhLevel).HasPrecision(4, 2);
        builder.Property(i => i.BulkDensityKgPerM3).HasPrecision(7, 2);
    }
}
