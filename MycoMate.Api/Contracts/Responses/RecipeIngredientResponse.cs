namespace MycoMate.Api.Contracts.Responses;

public record RecipeIngredientResponse(Guid IngredientId, string ShortName, string DisplayName, decimal MoistureContent, decimal WetAmount, decimal WetAmountPercent);
