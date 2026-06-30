using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class AminoAcidConfiguration : IEntityTypeConfiguration<AminoAcid>
{
    public void Configure(EntityTypeBuilder<AminoAcid> builder)
    {
        builder.HasKey(a => a.Id);
        builder.HasIndex(a => a.ShortName).IsUnique();
    }
}
