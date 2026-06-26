namespace MycoMate.Api.Contracts.Responses;

public record ProjectResponse(
    Guid Id,
    string Name,
    DateTime Created,
    string OwnerId
);
