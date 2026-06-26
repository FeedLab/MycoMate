using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Models;

namespace MycoMate.Api.Data;

public class MycoMateDbContext(DbContextOptions<MycoMateDbContext> options) : IdentityDbContext<IdentityUser>(options)
{
    public DbSet<Ingredient> Ingredients => Set<Ingredient>();
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<ProjectMember> ProjectMembers => Set<ProjectMember>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.ApplyConfigurationsFromAssembly(typeof(MycoMateDbContext).Assembly);
    }
}
