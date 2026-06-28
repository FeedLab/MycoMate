namespace MycoMate.Api.Models;

public class RecipeIngredient
{
    public Guid RecipeId { get; set; }
    public SubstrateRecipe Recipe { get; set; } = null!;

    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal Amount { get; set; }
}
