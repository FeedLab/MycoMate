using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace MycoMate.Maui.Models;

public enum ProjectRole { Owner, Editor, Viewer }

public partial class Project : ObservableObject
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public ProjectRole Role { get; set; }
    public ObservableCollection<SubstrateRecipe> Recipes { get; } = [];

    [ObservableProperty]
    bool isExpanded;

    public string RoleSymbol => Role switch
    {
        ProjectRole.Owner  => "\u2605", // ★
        ProjectRole.Editor => "\u270e", // ✎
        _                  => "\u25ce"  // ◎
    };
}
