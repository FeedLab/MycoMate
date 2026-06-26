using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MycoMate.Api.Models;

public class ProjectMember
{
    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    [MaxLength(450)] public required string UserId { get; set; }
    public IdentityUser User { get; set; } = null!;

    public ProjectRole Role { get; set; }
}
