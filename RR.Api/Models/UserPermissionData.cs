using RR.Api.Auth;

namespace RR.Api.Models;

/// <summary>
/// Read-only response DTO containing a user's roles and permissions.
/// Collections are <see cref="IReadOnlySet{T}"/> to prevent callers from mutating cached data.
/// </summary>
public sealed class UserPermissionData
{
    public required IReadOnlySet<AppRole> Roles { get; init; }

    public required IReadOnlySet<AppPermission> Permissions { get; init; }
}
