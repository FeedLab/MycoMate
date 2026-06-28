using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using MycoMate.Maui.Messages;
using MycoMate.Maui.Models;
using MycoMate.Maui.Services.Projects;

namespace MycoMate.Maui.ViewModels;

public partial class ProjectsViewModel : ObservableObject
{
    public ObservableCollection<Project> Projects { get; } = [];

    [ObservableProperty]
    bool isLoading;

    [ObservableProperty]
    Project? selectedProject;

    private readonly ProjectService projectService;
    private readonly ILogger<ProjectsViewModel> logger;

    /// <inheritdoc/>
    public ProjectsViewModel(ProjectService projectService, ILogger<ProjectsViewModel> logger)
    {
        this.projectService = projectService;
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
                    
                    foreach (var p in result)
                    {
                        Projects.Add(new Project { Id = p.Id, Name = p.Name });
                    }
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

    // [RelayCommand]
    // async Task LoadAsync()
    // {
    //     IsLoading = true;
    //     try
    //     {
    //         var result = await projectService.GetAllAsync();
    //         Projects.Clear();
    //         foreach (var p in result)
    //             Projects.Add(new Project { Id = p.Id, Name = p.Name });
    //     }
    //     catch (Exception ex)
    //     {
    //         logger.LogError(ex, "Failed to load projects");
    //     }
    //     finally
    //     {
    //         IsLoading = false;
    //     }
    // }

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
            // TODO: call update endpoint once available in API
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
}
