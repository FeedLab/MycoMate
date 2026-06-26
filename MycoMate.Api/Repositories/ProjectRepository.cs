using Microsoft.EntityFrameworkCore;
using MycoMate.Api.Data;
using MycoMate.Api.Models;

namespace MycoMate.Api.Repositories;

public class ProjectRepository(MycoMateDbContext db, ILogger<ProjectRepository> logger) : IProjectRepository
{
    public async Task<Project> AddAsync(Project project, CancellationToken ct = default)
    {
        logger.LogInformation("Adding project {Name} for owner {OwnerId}", project.Name, project.OwnerId);

        project.Members.Add(new ProjectMember
        {
            ProjectId = project.Id,
            UserId = project.OwnerId,
            Role = ProjectRole.Owner
        });

        db.Projects.Add(project);
        await db.SaveChangesAsync(ct);

        logger.LogInformation("Project {Id} saved successfully", project.Id);

        return project;
    }

    public async Task<IEnumerable<Project>> GetVisibleToUserAsync(string userId, CancellationToken ct = default)
    {
        return await db.Projects
            .Where(p => p.Members.Any(m => m.UserId == userId))
            .ToListAsync(ct);
    }

    public async Task<Project?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await db.Projects.FindAsync([id], ct);
    }

    public async Task<ProjectRole?> GetUserRoleAsync(Guid projectId, string userId, CancellationToken ct = default)
    {
        return await db.ProjectMembers
            .Where(m => m.ProjectId == projectId && m.UserId == userId)
            .Select(m => (ProjectRole?)m.Role)
            .FirstOrDefaultAsync(ct);
    }

    public async Task AddMemberAsync(Guid projectId, string userId, ProjectRole role, CancellationToken ct = default)
    {
        var existing = await db.ProjectMembers
            .FirstOrDefaultAsync(m => m.ProjectId == projectId && m.UserId == userId, ct);

        if (existing is not null)
        {
            existing.Role = role;
        }
        else
        {
            db.ProjectMembers.Add(new ProjectMember { ProjectId = projectId, UserId = userId, Role = role });
        }

        await db.SaveChangesAsync(ct);
    }

    public async Task<bool> RemoveMemberAsync(Guid projectId, string userId, CancellationToken ct = default)
    {
        var rows = await db.ProjectMembers
            .Where(m => m.ProjectId == projectId && m.UserId == userId)
            .ExecuteDeleteAsync(ct);

        return rows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, string ownerId, CancellationToken ct = default)
    {
        var rows = await db.Projects
            .Where(p => p.Id == id && p.OwnerId == ownerId)
            .ExecuteDeleteAsync(ct);

        return rows > 0;
    }
}
