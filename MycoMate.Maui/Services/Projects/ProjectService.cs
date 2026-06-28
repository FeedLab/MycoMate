using MycoMate.Maui.Api;
using Refit;

namespace MycoMate.Maui.Services.Projects;

public class ProjectService(IMycoMateApiv1 api)
{
    public async Task GetAllAsync()
    {
        try
        {
            await api.GetProjects();
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to get projects: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task GetAsync(Guid id)
    {
        try
        {
            await api.GetProject(id);
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to get project: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task CreateAsync(string name)
    {
        try
        {
            await api.CreateProject(new CreateProjectRequest { Name = name });
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to create project: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task AddMemberAsync(Guid projectId, string userId, int role)
    {
        try
        {
            await api.AddProjectMember(projectId, new AddMemberRequest { UserId = userId, Role = role });
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to add member: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task RemoveMemberAsync(Guid projectId, string memberId)
    {
        try
        {
            await api.RemoveProjectMember(projectId, memberId);
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to remove member: {ex.ReasonPhrase}", ex);
        }
    }

    public async Task DeleteAsync(Guid projectId)
    {
        try
        {
            await api.DeleteProject(projectId);
        }
        catch (ApiException ex)
        {
            throw new ServiceException($"Failed to delete project: {ex.ReasonPhrase}", ex);
        }
    }
}
