using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data.Configurations;

public class VitaminConfiguration : IEntityTypeConfiguration<Vitamin>
{
    public void Configure(EntityTypeBuilder<Vitamin> builder)
    {
        builder.HasKey(v => v.Id);
        builder.HasIndex(v => v.ShortName).IsUnique();
    }
}
