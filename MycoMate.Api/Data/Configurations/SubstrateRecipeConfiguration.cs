using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class SubstrateRecipeConfiguration : IEntityTypeConfiguration<SubstrateRecipe>
{
    public void Configure(EntityTypeBuilder<SubstrateRecipe> builder)
    {
        builder.HasKey(r => r.Id);

        builder.HasOne(r => r.Project)
            .WithMany()
            .HasForeignKey(r => r.ProjectId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
