using MycoMate.Maui.Models;
using MycoMate.Maui.Services;
using MycoMate.Maui.ViewModels;


namespace MycoMate.Maui.Views.Projects;

public partial class ProjectsView : ContentView
{
    private Project? editingProject;

    public ProjectsView()
    {
        InitializeComponent();

        var vm = AppService.GetRequiredService<ProjectsViewModel>();
        BindingContext = vm;

        vm.Projects.CollectionChanged += (_, _) => EmptyState.IsVisible = vm.Projects.Count == 0;
    }

    private void OnHeaderTapped(object sender, TappedEventArgs e)
    {
        if (sender is BindableObject { BindingContext: Project project })
            project.IsExpanded = !project.IsExpanded;
    }

    private void OnRecipeTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject { BindingContext: SubstrateRecipe recipe }) return;
        if (BindingContext is not ProjectsViewModel vm) return;
        vm.SelectRecipeCommand.Execute(recipe);
    }

    private void OnAddClicked(object sender, EventArgs e)
    {
        editingProject = null;
        DialogTitle.Text = "New Project";
        ProjectNameEntry.Text = string.Empty;
        DialogOverlay.IsVisible = true;
        ProjectNameEntry.Focus();
    }

    private async void OnDeleteSwiped(object sender, EventArgs e)
    {
        if (sender is not SwipeItem { BindingContext: Project project }) return;
        if (BindingContext is not ProjectsViewModel vm) return;

        var confirmed = await Application.Current!.Windows[0].Page!
            .DisplayAlertAsync("Delete project", $"Delete \"{project.Name}\"?", "Delete", "Cancel");

        if (!confirmed) return;

        try
        {
            await vm.DeleteCommand.ExecuteAsync(project);
        }
        catch
        {
            await Application.Current!.Windows[0].Page!
                .DisplayAlertAsync("Error", "Could not delete project. Please try again.", "OK");
        }
    }

    private void OnDialogCancelClicked(object sender, EventArgs e)
    {
        DialogOverlay.IsVisible = false;
    }

    private async void OnDialogSaveClicked(object sender, EventArgs e)
    {
        var name = ProjectNameEntry.Text?.Trim();
        if (string.IsNullOrEmpty(name)) return;

        DialogOverlay.IsVisible = false;

        if (BindingContext is not ProjectsViewModel vm) return;

        try
        {
            if (editingProject is null)
                await vm.AddCommand.ExecuteAsync(name);
            else
                await vm.EditCommand.ExecuteAsync((editingProject.Id, name));
        }
        catch
        {
            await Application.Current!.Windows[0].Page!
                .DisplayAlertAsync("Error", "Could not save project. Please try again.", "OK");
        }
    }
}
