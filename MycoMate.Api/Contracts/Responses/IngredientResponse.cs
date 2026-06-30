namespace MycoMate.Api.Contracts.Responses;

public record IngredientResponse(
    Guid Id,
    string ShortName,
    string DisplayName,
    string? Information,
    decimal MoistureContent,
    decimal? CarbonToNitrogenRatio,
    decimal? PhLevel,
    string? Function,
    decimal? BulkDensityKgPerM3,
    List<NutrientValueResponse> Minerals,
    List<NutrientValueResponse> Vitamins,
    List<NutrientValueResponse> AminoAcids,
    DateTime Created
);
