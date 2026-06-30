using CommunityToolkit.Mvvm.ComponentModel;

namespace MycoMate.Maui.Models;

public partial class RecipeIngredient : ObservableObject
{
    public Guid IngredientId { get; set; }
    public string ShortName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>Ingredient moisture content (%).</summary>
    public decimal MoistureContent { get; set; }

    /// <summary>% of total recipe dry matter this ingredient contributes (from server).</summary>
    [ObservableProperty] decimal dryPercent;

    // Client-side calculated fields:

    /// <summary>Dry mass of this ingredient in kg.</summary>
    [ObservableProperty] decimal dryMatter;

    /// <summary>Wet mass of this ingredient in kg.</summary>
    [ObservableProperty] decimal wetMatter;

    /// <summary>Wet mass as % of total recipe wet weight.</summary>
    [ObservableProperty] decimal wetAmountPercent;
}
