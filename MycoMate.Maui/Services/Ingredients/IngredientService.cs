using MycoMate.Maui.Api;
using MycoMate.Maui.Models;
using Refit;

namespace MycoMate.Maui.Services.Ingredients;

public class IngredientService(IMycoMateApiv1 api)
{
    public async Task<IReadOnlyList<Ingredient>> GetAllAsync(Guid projectId)
    {
        try
        {
            var result = await api.GetIngredients(projectId);
            return result.Select(i => new Ingredient
            {
                Id = i.Id,
                ShortName = i.ShortName,
                DisplayName = i.DisplayName,
                MoistureContent = (decimal)i.MoistureContent
            }).ToList();
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to get ingredients: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task CreateAsync(Guid projectId, string shortName, string displayName, double moistureContent, string? information = null)
    {
        try
        {
            await api.CreateIngredient(projectId, new CreateIngredientRequest
            {
                ShortName = shortName,
                DisplayName = displayName,
                MoistureContent = moistureContent,
                Information = information!
            });
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to create ingredient: {ex.ReasonPhrase}", ex);
        }
    }
}
