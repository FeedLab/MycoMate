using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MycoMate.Maui.Messages;
using MycoMate.Maui.Models;
using MycoMate.Maui.Services.Auth;
using MycoMate.Maui.Services.Projects;
using MycoMate.Maui.Services.SubstrateRecipes;

namespace MycoMate.Maui.ViewModels;

public partial class ProjectsViewModel : ObservableObject
{
    public ObservableCollection<Project> Projects { get; } = [];

    [ObservableProperty]
    bool isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasSelectedRecipe))]
    SubstrateRecipe? selectedRecipe;

    public bool HasSelectedRecipe => SelectedRecipe is not null;

    private readonly ProjectService projectService;
    private readonly SubstrateRecipeService recipeService;
    private readonly TokenStore tokenStore;
    private readonly ILogger<ProjectsViewModel> logger;

    public ProjectsViewModel(ProjectService projectService, SubstrateRecipeService recipeService, TokenStore tokenStore, ILogger<ProjectsViewModel> logger)
    {
        this.projectService = projectService;
        this.recipeService = recipeService;
        this.tokenStore = tokenStore;
        this.logger = logger;

        CreateUserLoggedInSubscription();
    }

    private void CreateUserLoggedInSubscription()
    {
        WeakReferenceMessenger.Default.Register<UserLoggedInMessage>(this, (r, m) =>
        {
            MainThread.InvokeOnMainThreadAsync(async () =>
            {
                IsLoading = true;
                try
                {
                    var result = await projectService.GetAllAsync();
                    Projects.Clear();

                    var userId = tokenStore.UserId;
                    foreach (var p in result)
                    {
                        var project = new Project
                        {
                            Id = p.Id,
                            Name = p.Name,
                            Description = p.Description,
                            Role = p.OwnerId == userId ? ProjectRole.Owner : ProjectRole.Viewer
                        };

                        var recipes = await recipeService.GetAllAsync(project.Id);
                        foreach (var r in recipes)
                            project.Recipes.Add(r);

                        Projects.Add(project);
                    }

                    logger.LogInformation("Loaded {ProjectCount} projects", Projects.Count);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to load projects");
                }
                finally
                {
                    IsLoading = false;
                }
            });
        });
    }

    [RelayCommand]
    async Task AddAsync(string name)
    {
        if (string.IsNullOrWhiteSpace(name)) return;
        try
        {
            await projectService.CreateAsync(name);
            Projects.Add(new Project { Id = Guid.NewGuid(), Name = name });
            logger.LogInformation("Project created: {Name}", name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create project {Name}", name);
            throw;
        }
    }

    [RelayCommand]
    async Task DeleteAsync(Project project)
    {
        try
        {
            await projectService.DeleteAsync(project.Id);
            Projects.Remove(project);
            logger.LogInformation("Project deleted: {Id}", project.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to delete project {Id}", project.Id);
            throw;
        }
    }

    [RelayCommand]
    async Task EditAsync((Guid Id, string Name) args)
    {
        var project = Projects.FirstOrDefault(p => p.Id == args.Id);
        if (project is null) return;
        try
        {
            project.Name = args.Name;
            var index = Projects.IndexOf(project);
            Projects[index] = project;
            logger.LogInformation("Project renamed: {Id} → {Name}", args.Id, args.Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to edit project {Id}", args.Id);
            throw;
        }
    }

    [RelayCommand]
    void SelectRecipe(SubstrateRecipe recipe)
    {
        if (SelectedRecipe is not null)
            SelectedRecipe.IsSelected = false;
        SelectedRecipe = recipe;
        recipe.IsSelected = true;
    }

    [RelayCommand]
    async Task LoadRecipesAsync(Project project)
    {
        try
        {
            var recipes = await recipeService.GetAllAsync(project.Id);
            project.Recipes.Clear();
            foreach (var r in recipes)
                project.Recipes.Add(r);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load recipes for project {Id}", project.Id);
        }
    }
}
