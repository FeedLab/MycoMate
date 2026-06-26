namespace MycoMate.Api.Contracts.Requests;

public record CreateIngredientRequest(
    string ShortName,
    string DisplayName,
    string? Information,
    decimal MoistureContent
);
