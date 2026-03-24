using RR.Common.Auth;

namespace RR.Model;

/// <summary>
/// Internal mutable model for caching a user's permissions in memory.
/// Not exposed outside the service layer.
/// </summary>
public sealed class CachedUserPermissions
{
    public required HashSet<AppRole> Roles { get; init; }

    public required HashSet<AppPermission> Permissions { get; init; }
}
