using MycoMate.Maui.Api;
using Refit;

namespace MycoMate.Maui.Services.Ingredients;

public class IngredientService(IMycoMateApiv1 api)
{
    public async Task CreateAsync(string shortName, string displayName, double moistureContent, string? information = null)
    {
        try
        {
            await api.CreateIngredient(new CreateIngredientRequest
            {
                ShortName = shortName,
                DisplayName = displayName,
                MoistureContent = moistureContent,
                Information = information
            });
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to create ingredient: {ex.ReasonPhrase}", ex);
        }
    }
}
