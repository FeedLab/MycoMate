using MycoMate.Maui.Models;

namespace MycoMate.Maui.Views.Projects;

public class TreeItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate ProjectTemplate { get; set; } = null!;
    public DataTemplate RecipeTemplate { get; set; } = null!;

    protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
    {
        System.Diagnostics.Debug.WriteLine($"[TreeItemTemplateSelector] item type={item?.GetType().Name ?? "null"}");
        return item is Project ? ProjectTemplate : RecipeTemplate;
    }
}
