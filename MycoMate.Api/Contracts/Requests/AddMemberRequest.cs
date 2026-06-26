using MycoMate.Api.Models;

namespace MycoMate.Api.Contracts.Requests;

public record AddMemberRequest(string UserId, ProjectRole Role);
