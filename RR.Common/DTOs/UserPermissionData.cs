using RR.Common.Auth;

namespace RR.Common.DTOs;

/// <summary>
/// Response DTO containing a user's roles and permissions.
/// Used by both the API (as a response) and the UI (as a deserialized model).
/// </summary>
public sealed class UserPermissionData
{
    public required HashSet<AppRole> Roles { get; init; }

    public required HashSet<AppPermission> Permissions { get; init; }
}
