using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class MineralConfiguration : IEntityTypeConfiguration<Mineral>
{
    public void Configure(EntityTypeBuilder<Mineral> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => m.ShortName).IsUnique();
    }
}
