using RR.Common.Auth;

namespace RR.UI.Auth;

/// <summary>
/// Client-side model containing the authenticated user's roles and permissions.
/// Deserialized from the backend <c>GET /api/permissions</c> response.
/// All collections use <see cref="HashSet{T}"/> for O(1) lookups.
/// </summary>
public sealed class UserPermissions
{
    public HashSet<AppRole> Roles { get; init; } = [];

    public HashSet<AppPermission> Permissions { get; init; } = [];

    /// <summary>Checks if the user has the specified role.</summary>
    public bool HasRole(AppRole role) => Roles.Contains(role);

    /// <summary>Checks if the user has <b>any</b> of the specified roles.</summary>
    public bool HasAnyRole(params AppRole[] roles) => roles.Any(Roles.Contains);

    /// <summary>Checks if the user has <b>all</b> of the specified roles.</summary>
    public bool HasAllRoles(params AppRole[] roles) => roles.All(Roles.Contains);

    /// <summary>Checks if the user has the specified permission.</summary>
    public bool HasPermission(AppPermission permission) => Permissions.Contains(permission);

    /// <summary>Checks if the user has <b>any</b> of the specified permissions.</summary>
    public bool HasAnyPermission(params AppPermission[] permissions) => permissions.Any(Permissions.Contains);

    /// <summary>Checks if the user has <b>all</b> of the specified permissions.</summary>
    public bool HasAllPermissions(params AppPermission[] permissions) => permissions.All(Permissions.Contains);
}
