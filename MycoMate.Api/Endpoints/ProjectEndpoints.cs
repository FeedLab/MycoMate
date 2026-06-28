using System.Security.Claims;
using MycoMate.Api.Contracts.Requests;
using MycoMate.Api.Contracts.Responses;
using MycoMate.Api.Models;
using MycoMate.Api.Repositories;

namespace MycoMate.Api.Endpoints;

public static class ProjectEndpoints
{
    public static IEndpointRouteBuilder MapProjectEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/projects")
            .WithTags("Projects")
            .RequireAuthorization();

        group.MapGet("/", async (ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var projects = await repo.GetVisibleToUserAsync(userId, ct);
                var response = projects.Select(p => new ProjectResponse(p.Id, p.Name, p.Created, p.OwnerId));

                return TypedResults.Ok(response);
            })
            .WithName("GetProjects");

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, IProjectRepository repo,
                CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await repo.GetUserRoleAsync(id, userId, ct);

                if (role is null)
                {
                    return Results.NotFound() as IResult;
                }

                var project = await repo.GetByIdAsync(id, ct);

                return TypedResults.Ok(new ProjectResponse(project!.Id, project.Name, project.Created, project.OwnerId));
            })
            .WithName("GetProject");

        group.MapPost("/", async (CreateProjectRequest req, ClaimsPrincipal user, IProjectRepository repo,
                CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

                var project = new Project { Name = req.Name, OwnerId = userId };

                await repo.AddAsync(project, ct);

                var response = new ProjectResponse(project.Id, project.Name, project.Created, project.OwnerId);

                return Results.Created($"/projects/{project.Id}", response);
            })
            .WithName("CreateProject");

        group.MapPost("/{projectId:guid}/members", async (Guid projectId, AddMemberRequest req,
                ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await repo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null)
                {
                    return Results.NotFound();
                }

                if (role < ProjectRole.Owner)
                {
                    return Results.Forbid();
                }

                await repo.AddMemberAsync(projectId, req.UserId, req.Role, ct);

                return Results.NoContent();
            })
            .WithName("AddProjectMember");

        group.MapDelete("/{projectId:guid}/members/{memberId}", async (Guid projectId, string memberId,
                ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await repo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null)
                {
                    return Results.NotFound();
                }

                if (role < ProjectRole.Owner)
                {
                    return Results.Forbid();
                }

                var removed = await repo.RemoveMemberAsync(projectId, memberId, ct);

                if (!removed)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            })
            .WithName("RemoveProjectMember");

        group.MapDelete("/{projectId:guid}", async (Guid projectId, ClaimsPrincipal user,
                IProjectRepository repo, CancellationToken ct) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
                var role = await repo.GetUserRoleAsync(projectId, userId, ct);

                if (role is null || role < ProjectRole.Owner)
                {
                    return Results.Forbid();
                }

                var deleted = await repo.DeleteAsync(projectId, userId, ct);

                if (!deleted)
                {
                    return Results.NotFound();
                }

                return Results.NoContent();
            })
            .WithName("DeleteProject");

        return app;
    }
}
