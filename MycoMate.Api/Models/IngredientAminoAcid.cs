namespace MycoMate.Api.Models;

public class IngredientAminoAcid
{
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public Guid AminoAcidId { get; set; }
    public AminoAcid AminoAcid { get; set; } = null!;
    public decimal Value { get; set; }
}
