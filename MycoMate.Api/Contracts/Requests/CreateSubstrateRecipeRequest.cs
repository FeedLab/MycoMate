namespace MycoMate.Api.Contracts.Requests;

public record CreateSubstrateRecipeRequest(string Name, string? Description, decimal MoistureContentTarget, decimal FinalMixtureSizeKg);
