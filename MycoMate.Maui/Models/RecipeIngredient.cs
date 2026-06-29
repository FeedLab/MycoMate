namespace MycoMate.Maui.Models;

public class RecipeIngredient
{
    public Guid IngredientId { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public decimal MoistureContent { get; set; }
    public decimal WetAmount { get; set; }
    public decimal WetAmountPercent { get; set; }
}
