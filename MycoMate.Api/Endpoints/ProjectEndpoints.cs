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
            return Results.Ok(response);
        })
        .WithName("GetProjects");

        group.MapGet("/{id:guid}", async (Guid id, ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var role = await repo.GetUserRoleAsync(id, userId, ct);
            if (role is null) return Results.NotFound();

            var project = await repo.GetByIdAsync(id, ct);
            return Results.Ok(new ProjectResponse(project!.Id, project.Name, project.Created, project.OwnerId));
        })
        .WithName("GetProject");

        group.MapPost("/", async (CreateProjectRequest req, ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;

            var project = new Project { Name = req.Name, OwnerId = userId };
            await repo.AddAsync(project, ct);

            var response = new ProjectResponse(project.Id, project.Name, project.Created, project.OwnerId);
            return Results.Created($"/projects/{project.Id}", response);
        })
        .WithName("CreateProject");

        group.MapPost("/{id:guid}/members", async (Guid id, AddMemberRequest req, ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var role = await repo.GetUserRoleAsync(id, userId, ct);
            if (role is null) return Results.NotFound();
            if (role < ProjectRole.Owner) return Results.Forbid();

            await repo.AddMemberAsync(id, req.UserId, req.Role, ct);
            return Results.NoContent();
        })
        .WithName("AddProjectMember");

        group.MapDelete("/{id:guid}/members/{memberId}", async (Guid id, string memberId, ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var role = await repo.GetUserRoleAsync(id, userId, ct);
            if (role is null) return Results.NotFound();
            if (role < ProjectRole.Owner) return Results.Forbid();

            var removed = await repo.RemoveMemberAsync(id, memberId, ct);
            return removed ? Results.NoContent() : Results.NotFound();
        })
        .WithName("RemoveProjectMember");

        group.MapDelete("/{id:guid}", async (Guid id, ClaimsPrincipal user, IProjectRepository repo, CancellationToken ct) =>
        {
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var role = await repo.GetUserRoleAsync(id, userId, ct);
            if (role is null || role < ProjectRole.Owner) return Results.Forbid();

            var deleted = await repo.DeleteAsync(id, userId, ct);
            return deleted ? Results.NoContent() : Results.NotFound();
        })
        .WithName("DeleteProject");

        return app;
    }
}
