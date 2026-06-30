namespace MycoMate.Api.Contracts.Responses;

public record NutrientValueResponse(
    Guid NutrientId,
    string Name,
    string ShortName,
    string? Description,
    decimal Value,
    string Unit
);
