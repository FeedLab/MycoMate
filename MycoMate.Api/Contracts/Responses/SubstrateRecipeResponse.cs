namespace MycoMate.Api.Contracts.Responses;

public record SubstrateRecipeResponse(
    Guid Id,
    string Name,
    string? Description,
    decimal MoistureContentTarget,
    decimal FinalMixtureSizeKg,
    DateTime Created,
    Guid ProjectId,
    IEnumerable<RecipeIngredientResponse> Ingredients);
