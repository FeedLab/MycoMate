namespace MycoMate.Api.Contracts.Responses;

public record IngredientResponse(
    Guid Id,
    string ShortName,
    string DisplayName,
    string? Information,
    decimal MoistureContent,
    DateTime Created
);
