using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MycoMate.Maui.Models;

public partial class SubstrateRecipe : ObservableObject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }

    /// <summary>Target moisture content for the whole batch (%).</summary>
    public decimal MoistureContentTarget { get; set; }

    /// <summary>Total wet weight of the finished batch (kg).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WaterKg))]
    decimal finalMixtureSizeKg;

    /// <summary>Water to add (+) or remove (–) as % of FinalMixtureSizeKg (server-calculated).</summary>
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(WaterKg))]
    decimal waterAdjustmentPercent;

    /// <summary>Actual water to add/remove in kg.</summary>
    public decimal WaterKg => FinalMixtureSizeKg * WaterAdjustmentPercent / 100m;

    [ObservableProperty] bool isSelected;

    public ObservableCollection<RecipeIngredient> Ingredients { get; init; } = [];

    partial void OnFinalMixtureSizeKgChanged(decimal value)
    {
        var totalDryKg = value * (1m - MoistureContentTarget / 100m);
        foreach (var i in Ingredients)
        {
            var dryMatter = Math.Round(totalDryKg * i.DryPercent / 100m, 3);
            var divisor = 1m - i.MoistureContent / 100m;
            var wetMatter = divisor > 0 ? Math.Round(dryMatter / divisor, 3) : 0m;
            i.DryMatter = dryMatter;
            i.WetMatter = wetMatter;
            i.WetAmountPercent = value > 0 ? Math.Round(wetMatter / value * 100m, 2) : 0m;
        }
    }
}
