using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MycoMate.Api.Models;

public class Project
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [MaxLength(100)] public required string Name { get; set; }
    [MaxLength(500)] public string? Description { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;

    [MaxLength(450)] public required string OwnerId { get; set; }
    public IdentityUser Owner { get; set; } = null!;

    public ICollection<ProjectMember> Members { get; set; } = [];
}
