namespace MycoMate.Api.Contracts.Responses;

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime Created,
    string OwnerId
);
