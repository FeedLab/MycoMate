using System.Collections.ObjectModel;
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
            return result.Select(ToModel).ToList();
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to get recipes: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task<IReadOnlyList<RecipeIngredient>> SetIngredientAsync(
        Guid projectId, Guid recipeId, Guid ingredientId, decimal dryPercent,
        decimal finalMixtureSizeKg, decimal moistureContentTarget)
    {
        try
        {
            var result = await api.SetRecipeIngredient(
                projectId, recipeId, ingredientId,
                new RecipeIngredientRequest { DryPercent = (double)dryPercent });
            return result.Ingredients.Select(r => ToIngredientModel(r, finalMixtureSizeKg, moistureContentTarget)).ToList();
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to set ingredient: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task RemoveIngredientAsync(Guid projectId, Guid recipeId, Guid ingredientId)
    {
        try
        {
            await api.RemoveRecipeIngredient(projectId, recipeId, ingredientId);
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to remove ingredient: {ex.ReasonPhrase}", ex);
        }
    }

    private static SubstrateRecipe ToModel(SubstrateRecipeResponse r)
    {
        var finalKg = (decimal)r.FinalMixtureSizeKg;
        var moistureTarget = (decimal)r.MoistureContentTarget;

        return new SubstrateRecipe
        {
            Id = r.Id,
            Name = r.Name,
            Description = r.Description,
            ProjectId = r.ProjectId,
            MoistureContentTarget = moistureTarget,
            FinalMixtureSizeKg = finalKg,
            WaterAdjustmentPercent = (decimal)r.WaterAdjustmentPercent,
            Ingredients = new ObservableCollection<RecipeIngredient>(
                    r.Ingredients
                        .Select(i => ToIngredientModel(i, finalKg, moistureTarget))
                        .OrderByDescending(o => o.MoistureContent)
                        .ToList()
                    )
 
        };
    }

    private static RecipeIngredient ToIngredientModel(
        RecipeIngredientResponse i, decimal finalMixtureSizeKg, decimal moistureContentTarget)
    {
        var amountInPercentOfDryMatter = (decimal)i.DryPercent;
        var ingredientMoistureContent = (decimal)i.MoistureContent;

        // Total dry matter in the batch
        var totalDryKg = finalMixtureSizeKg * (1m - moistureContentTarget / 100m);

        // This ingredient's share of dry matter
        var ingredientDryMatter = Math.Round(totalDryKg * amountInPercentOfDryMatter / 100m, 3);

        // Wet weight: DryMatter / (1 - MoistureContent%)
        var divisor = 1m - ingredientMoistureContent / 100m;
        var ingredientWetMatter = divisor > 0 ? Math.Round(ingredientDryMatter / divisor, 3) : 0m;

        // Wet weight as % of total wet batch
        var wetAmountPercent = finalMixtureSizeKg > 0
            ? Math.Round(ingredientWetMatter / finalMixtureSizeKg * 100m, 2)
            : 0m;

        return new RecipeIngredient
        {
            IngredientId = i.IngredientId,
            ShortName = i.ShortName,
            DisplayName = i.DisplayName,
            MoistureContent = ingredientMoistureContent,
            DryPercent = amountInPercentOfDryMatter,
            DryMatter = ingredientDryMatter,
            WetMatter = ingredientWetMatter,
            WetAmountPercent = wetAmountPercent
        };
    }
}