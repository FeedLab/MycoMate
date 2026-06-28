using System.ComponentModel.DataAnnotations;

namespace MycoMate.Api.Models;

public class SubstrateRecipe
{
    public Guid Id { get; init; } = Guid.NewGuid();
    [MaxLength(100)] public required string Name { get; set; }
    [MaxLength(1000)] public string? Description { get; set; }
    public decimal MoistureContentTarget { get; set; }
    public decimal FinalMixtureSizeKg { get; set; }
    public DateTime Created { get; init; } = DateTime.UtcNow;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;

    public ICollection<RecipeIngredient> Ingredients { get; set; } = [];
}
