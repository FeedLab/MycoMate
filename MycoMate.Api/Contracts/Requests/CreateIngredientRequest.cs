namespace MycoMate.Api.Contracts.Requests;

public record CreateIngredientRequest(
    string ShortName,
    string DisplayName,
    string? Information,
    decimal MoistureContent,
    decimal? CarbonToNitrogenRatio,
    decimal? PhLevel,
    string? Function,
    decimal? BulkDensityKgPerM3,
    List<NutrientValueRequest>? Minerals,
    List<NutrientValueRequest>? Vitamins,
    List<NutrientValueRequest>? AminoAcids
);
