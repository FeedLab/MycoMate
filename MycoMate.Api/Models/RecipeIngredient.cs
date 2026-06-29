namespace MycoMate.Api.Models;

public class RecipeIngredient
{
    public Guid RecipeId { get; set; }
    public SubstrateRecipe Recipe { get; set; } = null!;

    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public decimal WetAmount { get; set; }
    public decimal WetAmountPercent { get; set; }
    public decimal WetMatter { get; set; }
    public decimal MoistureContent { get; set; }
    public decimal DryMatter { get; set; }
    public decimal DryAmountPercent { get; set; }
}
