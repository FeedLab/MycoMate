namespace MycoMate.Api.Models;

public class IngredientVitamin
{
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public Guid VitaminId { get; set; }
    public Vitamin Vitamin { get; set; } = null!;
    public decimal Value { get; set; }
}
