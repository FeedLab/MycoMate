using MycoMate.Api.Models;

namespace MycoMate.Api.Services;

/// <summary>
/// Calculates water-related properties of a substrate recipe.
/// - WetAmountPercent : share of FinalMixtureSizeKg this ingredient occupies (wet-weight basis)
/// - MoistureContent  : wet-basis % (water / wet weight × 100), snapshotted at time of adding
/// Requires the recipe's Ingredients collection to be loaded.
/// </summary>
public class SubstrateRecipeCalculator(SubstrateRecipe recipe)
{
    private static decimal WetWeightKg(SubstrateRecipe r, RecipeIngredient ri) =>
        r.FinalMixtureSizeKg * (ri.WetAmountPercent / 100m);

    private static decimal WaterFromIngredient(decimal wetWeightKg, decimal moistureContentPct) =>
        wetWeightKg * (moistureContentPct / 100m);

    /// <summary>
    /// Total water already present in the ingredients (kg).
    /// </summary>
    public decimal WaterContentKg() =>
        recipe.Ingredients.Sum(ri =>
            WaterFromIngredient(WetWeightKg(recipe, ri), ri.MoistureContent));

    /// <summary>
    /// Water to add (kg) so the final mixture reaches MoistureContentTarget.
    /// Returns 0 if existing ingredients already meet or exceed the target.
    /// </summary>
    public decimal WaterToAddKg()
    {
        var targetWaterKg = recipe.FinalMixtureSizeKg * (recipe.MoistureContentTarget / 100m);
        return Math.Max(0m, targetWaterKg - WaterContentKg());
    }
}
