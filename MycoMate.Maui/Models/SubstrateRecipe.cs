using CommunityToolkit.Mvvm.ComponentModel;

namespace MycoMate.Maui.Models;

public partial class SubstrateRecipe : ObservableObject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid ProjectId { get; set; }
    public List<RecipeIngredient> Ingredients { get; init; } = [];

    [ObservableProperty]
    bool isSelected;
}
