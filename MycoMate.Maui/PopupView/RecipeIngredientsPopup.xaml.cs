using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using MycoMate.Maui.Models;
using MycoMate.Maui.ViewModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.Inputs;

namespace MycoMate.Maui.PopupView;

public partial class RecipeIngredientsPopup : Popup
{
    private readonly ProjectsViewModel vm;

    public RecipeIngredientsPopup(SubstrateRecipe recipe, ProjectsViewModel vm)
    {
        InitializeComponent();
        this.vm = vm;
        BindingContext = recipe;

        var window = Application.Current!.Windows[0];
        //ContentBorder.WidthRequest = window.Width * 0.8;
        //ContentBorder.HeightRequest = window.Height * 0.85;

        ContentBorder.SizeChanged += OnBorderSizeChanged;

        BatchSizeEntry.Value = (double)recipe.FinalMixtureSizeKg;
    }

    private void OnBorderSizeChanged(object sender, EventArgs e)
    {
        if (ContentBorder.Width <= 0) return;
        const double gridMargin = 40; // Margin="20,8" → 20px each side
        const double mcColWidth = 90;
        const double wetColWidth = 130;
        var gridWidth = ContentBorder.Width - gridMargin;
        //IngredientsGrid.WidthRequest = gridWidth;
        //IngredientsGrid.Columns[0].Width = gridWidth - mcColWidth - wetColWidth;
        //IngredientsGrid.Columns[1].Width = mcColWidth;
        //IngredientsGrid.Columns[2].Width = wetColWidth;
    }

    private void OnMcChanged(object sender, NumericEntryValueChangedEventArgs e)
    {
        if (sender is not SfNumericEntry { BindingContext: RecipeIngredient ingredient }) return;
        if (BindingContext is not SubstrateRecipe recipe) return;

        var newMc = (decimal)(double)(e.NewValue ?? 0.0);
        ingredient.MoistureContent = newMc;

        var divisor = 1m - newMc / 100m;
        var wetMatter = divisor > 0 ? Math.Round(ingredient.DryMatter / divisor, 3) : 0m;
        ingredient.WetMatter = wetMatter;
        ingredient.WetAmountPercent = recipe.FinalMixtureSizeKg > 0
            ? Math.Round(wetMatter / recipe.FinalMixtureSizeKg * 100m, 2) : 0m;

        recipe.WaterAdjustmentPercent = recipe.FinalMixtureSizeKg > 0
            ? Math.Round((recipe.FinalMixtureSizeKg - recipe.Ingredients.Sum(i => i.WetMatter)) / recipe.FinalMixtureSizeKg * 100m, 3)
            : 0m;
    }

    private void OnBatchSizeChanged(object sender, NumericEntryValueChangedEventArgs e)
    {
        if (BindingContext is not SubstrateRecipe recipe) return;
        recipe.FinalMixtureSizeKg = (decimal)(double)(e.NewValue ?? 0.0);
    }

    private async void OnAddIngredientClicked(object sender, EventArgs e)
    {
        var available = await vm.GetAvailableIngredientsAsync();
        if (available.Count == 0)
        {
            await Application.Current!.Windows[0].Page!
                .DisplayAlertAsync("No ingredients", "No ingredients are available for this project.", "OK");
            return;
        }

        var result = await ShowIngredientPopupAsync(available);
        if (result?.Action != IngredientPopupAction.Save) return;

        try
        {
            await vm.SetIngredientAsync(result.IngredientId, result.DryPercent);
        }
        catch
        {
            await Application.Current!.Windows[0].Page!
                .DisplayAlertAsync("Error", "Could not add ingredient. Please try again.", "OK");
        }
    }

    private async void OnIngredientCellTapped(object sender, DataGridCellTappedEventArgs e)
    {
        if (e.RowData is not RecipeIngredient ingredient) return;

        var result = await ShowIngredientPopupAsync(ingredient);
        if (result is null || result.Action == IngredientPopupAction.Cancel) return;

        try
        {
            if (result.Action == IngredientPopupAction.Save)
                await vm.SetIngredientAsync(result.IngredientId, result.DryPercent);
            else if (result.Action == IngredientPopupAction.Delete)
                await vm.RemoveIngredientAsync(result.IngredientId);
        }
        catch
        {
            await Application.Current!.Windows[0].Page!
                .DisplayAlertAsync("Error", "Could not update ingredient. Please try again.", "OK");
        }
    }

    private async Task<IngredientPopupResult?> ShowIngredientPopupAsync(IReadOnlyList<Ingredient> available)
    {
        var tcs = new TaskCompletionSource<IngredientPopupResult?>();
        var popup = new IngredientPopup(available, r => tcs.TrySetResult(r));
        await Application.Current!.Windows[0].Page!.ShowPopupAsync(popup);
        tcs.TrySetResult(null);
        return await tcs.Task;
    }

    private async Task<IngredientPopupResult?> ShowIngredientPopupAsync(RecipeIngredient ingredient)
    {
        var tcs = new TaskCompletionSource<IngredientPopupResult?>();
        var popup = new IngredientPopup(ingredient, r => tcs.TrySetResult(r));
        await Application.Current!.Windows[0].Page!.ShowPopupAsync(popup);
        tcs.TrySetResult(null);
        return await tcs.Task;
    }

    private async void OnCloseClicked(object sender, EventArgs e)
    {
        await CloseAsync();
    }
}
