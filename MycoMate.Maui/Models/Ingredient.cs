namespace MycoMate.Maui.Models;

public class Ingredient
{
    public Guid Id { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string? Information { get; set; }
    public decimal MoistureContent { get; set; }
    public decimal? CarbonToNitrogenRatio { get; set; }
    public decimal? PhLevel { get; set; }
    public string? Function { get; set; }
    public decimal? BulkDensityKgPerM3 { get; set; }
    public List<NutrientEntry>? Minerals { get; set; }
    public List<NutrientEntry>? Vitamins { get; set; }
    public List<NutrientEntry>? AminoAcids { get; set; }
}
