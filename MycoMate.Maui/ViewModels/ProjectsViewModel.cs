using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MycoMate.Maui.Messages;
using MycoMate.Maui.Models;
using MycoMate.Maui.Services.Auth;
using MycoMate.Maui.Services.Ingredients;
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
    private readonly IngredientService ingredientService;
    private readonly TokenStore tokenStore;
    private readonly ILogger<ProjectsViewModel> logger;

    public ProjectsViewModel(ProjectService projectService, SubstrateRecipeService recipeService,
        IngredientService ingredientService, TokenStore tokenStore, ILogger<ProjectsViewModel> logger)
    {
        this.projectService = projectService;
        this.recipeService = recipeService;
        this.ingredientService = ingredientService;
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
                        foreach (var recipe in recipes)
                            project.Recipes.Add(recipe);

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

    public async Task<IReadOnlyList<Ingredient>> GetAvailableIngredientsAsync()
    {
        if (SelectedRecipe is null) return [];
        try
        {
            return await ingredientService.GetAllAsync(SelectedRecipe.ProjectId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to load ingredients for project {Id}", SelectedRecipe.ProjectId);
            return [];
        }
    }

    public async Task SetIngredientAsync(Guid ingredientId, decimal dryPercent)
    {
        if (SelectedRecipe is null) return;
        try
        {
            var updated = await recipeService.SetIngredientAsync(
                SelectedRecipe.ProjectId, SelectedRecipe.Id, ingredientId, dryPercent,
                SelectedRecipe.FinalMixtureSizeKg, SelectedRecipe.MoistureContentTarget);

            SelectedRecipe.Ingredients.Clear();
            foreach (var i in updated)
                SelectedRecipe.Ingredients.Add(i);

            logger.LogInformation("Set ingredient {IngredientId} on recipe {RecipeId}", ingredientId, SelectedRecipe.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to set ingredient {IngredientId}", ingredientId);
            throw;
        }
    }

    public async Task RemoveIngredientAsync(Guid ingredientId)
    {
        if (SelectedRecipe is null) return;
        try
        {
            await recipeService.RemoveIngredientAsync(
                SelectedRecipe.ProjectId, SelectedRecipe.Id, ingredientId);

            var toRemove = SelectedRecipe.Ingredients.FirstOrDefault(i => i.IngredientId == ingredientId);
            if (toRemove is not null)
                SelectedRecipe.Ingredients.Remove(toRemove);

            logger.LogInformation("Removed ingredient {IngredientId} from recipe {RecipeId}", ingredientId, SelectedRecipe.Id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove ingredient {IngredientId}", ingredientId);
            throw;
        }
    }
}
