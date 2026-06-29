using MycoMate.Maui.Api;
using MycoMate.Maui.Models;
using Refit;

namespace MycoMate.Maui.Services.SubstrateRecipes;

public class SubstrateRecipeService(IMycoMateApiv1 api)
{
    public async Task<ICollection<SubstrateRecipe>> GetAllAsync(Guid projectId)
    {
        try
        {
            var result = await api.GetSubstrateRecipes(projectId);
            return result.Select(r => new SubstrateRecipe
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                ProjectId = r.ProjectId,
                Ingredients = r.Ingredients.Select(i => new RecipeIngredient
                {
                    IngredientId = i.IngredientId,
                    ShortName = i.ShortName,
                    DisplayName = i.DisplayName,
                    MoistureContent = (decimal)i.MoistureContent,
                    WetAmount = (decimal)i.WetAmount,
                    WetAmountPercent = (decimal)i.WetAmountPercent
                }).ToList()
            }).ToList();
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to get recipes: {ex.ReasonPhrase}", ex);
        }
    }
}
