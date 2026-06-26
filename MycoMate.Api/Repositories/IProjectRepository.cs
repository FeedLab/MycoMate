using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public interface IProjectRepository
{
    Task<Project> AddAsync(Project project, CancellationToken ct = default);
    Task<IEnumerable<Project>> GetVisibleToUserAsync(string userId, CancellationToken ct = default);
    Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<ProjectRole?> GetUserRoleAsync(Guid projectId, string userId, CancellationToken ct = default);
    Task AddMemberAsync(Guid projectId, string userId, ProjectRole role, CancellationToken ct = default);
    Task<bool> RemoveMemberAsync(Guid projectId, string userId, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, string ownerId, CancellationToken ct = default);
}
