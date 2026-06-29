using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class SubstrateRecipeConfiguration : IEntityTypeConfiguration<SubstrateRecipe>
{
    public void Configure(EntityTypeBuilder<SubstrateRecipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.MoistureContentTarget)
            .HasPrecision(5, 2);

        builder.Property(r => r.FinalMixtureSizeKg)
            .HasPrecision(10, 3);

        builder.Property(r => r.WaterAdjustmentPercent)
            .HasPrecision(7, 4);

        builder.HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
