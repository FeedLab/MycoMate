namespace MycoMate.Api.Auth;

public static class Policies
{
    public const string CanCreateProject = nameof(CanCreateProject);
}

public static class Roles
{
    public const string Owner = nameof(Owner);
}

public static class RateLimitPolicies
{
    public const string Register = nameof(Register);
}
