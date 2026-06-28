namespace MycoMate.Api.Contracts.Requests;

public record UpdateSubstrateRecipeRequest(string Name, string? Description, decimal MoistureContentTarget, decimal FinalMixtureSizeKg);
