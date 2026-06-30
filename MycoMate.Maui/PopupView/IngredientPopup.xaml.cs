using CommunityToolkit.Maui.Views;
using MycoMate.Maui.Models;

namespace MycoMate.Maui.PopupView;

public enum IngredientPopupAction { Save, Delete, Cancel }

public record IngredientPopupResult(IngredientPopupAction Action, Guid IngredientId = default, decimal DryPercent = 0);

public partial class IngredientPopup : Popup
{
    private readonly RecipeIngredient? existing;
    private readonly Action<IngredientPopupResult> onResult;

    // Add mode
    public IngredientPopup(IReadOnlyList<Ingredient> availableIngredients, Action<IngredientPopupResult> onResult)
    {
        InitializeComponent();

        this.onResult = onResult;

        TitleLabel.Text = "Add Ingredient";
        PickerLayout.IsVisible = true;
        IngredientNameLayout.IsVisible = false;
        DeleteButton.IsVisible = false;

        IngredientPicker.ItemsSource = availableIngredients.ToList();
    }

    // Edit mode
    public IngredientPopup(RecipeIngredient ingredient, Action<IngredientPopupResult> onResult)
    {
        InitializeComponent();

        this.onResult = onResult;
        existing = ingredient;

        TitleLabel.Text = "Edit Ingredient";
        PickerLayout.IsVisible = false;
        IngredientNameLayout.IsVisible = true;
        DeleteButton.IsVisible = true;

        IngredientNameEntry.Text = ingredient.DisplayName;
        DryPercentEntry.Text = ingredient.DryPercent.ToString("F2");
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        if (!decimal.TryParse(DryPercentEntry.Text, System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture, out var dryPercent)
            || dryPercent <= 0 || dryPercent > 100)
        {
            await Application.Current!.Windows[0].Page!
                .DisplayAlertAsync("Validation", "Enter a valid dry percent between 0 and 100.", "OK");
            return;
        }

        Guid ingredientId;

        if (existing is not null)
        {
            ingredientId = existing.IngredientId;
        }
        else
        {
            if (IngredientPicker.SelectedItem is not Ingredient selected)
            {
                await Application.Current!.Windows[0].Page!
                    .DisplayAlertAsync("Validation", "Please select an ingredient.", "OK");
                return;
            }
            ingredientId = selected.Id;
        }

        onResult(new IngredientPopupResult(IngredientPopupAction.Save, ingredientId, dryPercent));
        await CloseAsync();
    }

    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        onResult(new IngredientPopupResult(IngredientPopupAction.Delete, existing!.IngredientId));
        await CloseAsync();
    }

    private async void OnCancelClicked(object sender, EventArgs e)
    {
        onResult(new IngredientPopupResult(IngredientPopupAction.Cancel));
        await CloseAsync();
    }
}
