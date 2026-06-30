namespace MycoMate.Api.Models;

public class IngredientMineral
{
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public Guid MineralId { get; set; }
    public Mineral Mineral { get; set; } = null!;
    public decimal Value { get; set; }
}
