using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class IngredientAminoAcidConfiguration : IEntityTypeConfiguration<IngredientAminoAcid>
{
    public void Configure(EntityTypeBuilder<IngredientAminoAcid> builder)
    {
        builder.HasKey(ia => new { ia.IngredientId, ia.AminoAcidId });

        builder.Property(ia => ia.Value).HasPrecision(10, 4);

        builder.HasOne(ia => ia.Ingredient)
            .WithMany(i => i.AminoAcids)
            .HasForeignKey(ia => ia.IngredientId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(ia => ia.AminoAcid)
            .WithMany()
            .HasForeignKey(ia => ia.AminoAcidId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
