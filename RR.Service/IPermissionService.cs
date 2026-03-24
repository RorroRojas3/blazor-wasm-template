using RR.Common.DTOs;

namespace RR.Service;

/// <summary>
/// Manages user permissions with an in-memory cache for fast lookups.
/// </summary>
public interface IPermissionService
{
    /// <summary>
    /// Loads all user permissions into memory. Called once at application startup.
    /// </summary>
    Task LoadAllAsync();

    /// <summary>
    /// Returns a read-only snapshot of the user's permissions,
    /// or <c>null</c> if the user has no entry in the store.
    /// </summary>
    UserPermissionData? GetPermissions(string userId);

    /// <summary>
    /// Clears and reloads all user permissions from the data store.
    /// </summary>
    Task ReloadAllAsync();

    /// <summary>
    /// Reloads a single user's permissions from the data store.
    /// </summary>
    Task ReloadUserAsync(string userId);

    /// <summary>
    /// Removes a single user from the in-memory cache.
    /// </summary>
    void InvalidateUser(string userId);

    /// <summary>
    /// Clears the entire in-memory permission cache.
    /// </summary>
    void InvalidateAll();
}
