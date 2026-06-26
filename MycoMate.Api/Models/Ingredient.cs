using System.ComponentModel.DataAnnotations;

namespace MycoMate.Api.Models;

public class Ingredient
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [MaxLength(30)]   public required string ShortName { get; set; }
    [MaxLength(50)]  public required string DisplayName { get; set; }
    [MaxLength(1000)] public string? Information { get; set; }
    public decimal MoistureContent { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;
    [MaxLength(35)]  public required string UserId { get; set; }
    public bool IsEnabled { get; set; } = true;
}
