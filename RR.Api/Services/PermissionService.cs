using System.Collections.Concurrent;
using System.Collections.Frozen;
using RR.Api.Auth;
using RR.Api.Models;

namespace RR.Api.Services;

/// <summary>
/// Singleton service that stores all user permissions in memory.
/// Data is loaded at application startup and served from a <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// so that lookups are synchronous and thread-safe.
/// </summary>
public sealed class PermissionService(ILogger<PermissionService> logger)
{
    /// <summary>
    /// Internal mutable storage per user. Never exposed outside this class.
    /// </summary>
    private sealed class MutableUserPermissions
    {
        public required HashSet<AppRole> Roles { get; init; }
        public required HashSet<AppPermission> Permissions { get; init; }
    }

    private readonly ConcurrentDictionary<string, MutableUserPermissions> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<PermissionService> _logger = logger;


    /// <summary>
    /// Loads all user permissions into memory. Called once at application startup.
    /// Replace the seed data below with real database calls in production.
    /// </summary>
    public Task LoadAllAsync()
    {
        _logger.LogInformation("Loading all user permissions into memory...");

        // ──────────────────────────────────────────────────────────────
        // SEED DATA — replace with real database / external store calls.
        // Keys are Entra Object IDs (the "oid" claim from the JWT).
        // ──────────────────────────────────────────────────────────────

        var seedData = new Dictionary<string, MutableUserPermissions>(StringComparer.OrdinalIgnoreCase)
        {
            // Sample admin user
            ["00000000-0000-0000-0000-000000000001"] = new()
            {
                Roles = [AppRole.Admin],
                Permissions =
                [
                    AppPermission.UsersRead,
                    AppPermission.UsersWrite,
                    AppPermission.UsersDelete,
                    AppPermission.ReportsView,
                    AppPermission.ReportsExport,
                    AppPermission.DashboardView,
                    AppPermission.SettingsManage
                ]
            },
            // Sample manager user
            ["00000000-0000-0000-0000-000000000002"] = new()
            {
                Roles = [AppRole.Manager, AppRole.Editor],
                Permissions =
                [
                    AppPermission.UsersRead,
                    AppPermission.UsersWrite,
                    AppPermission.ReportsView,
                    AppPermission.ReportsExport,
                    AppPermission.DashboardView
                ]
            },
            // Sample viewer user
            ["00000000-0000-0000-0000-000000000003"] = new()
            {
                Roles = [AppRole.Viewer],
                Permissions =
                [
                    AppPermission.ReportsView,
                    AppPermission.DashboardView
                ]
            }
        };

        foreach (var (userId, permissions) in seedData)
        {
            _cache[userId] = permissions;
        }

        _logger.LogInformation("Loaded permissions for {Count} users.", _cache.Count);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Synchronous lookup — returns a read-only snapshot of the user's permissions,
    /// or <c>null</c> if the user has no entry in the store.
    /// </summary>
    public UserPermissionData? GetPermissions(string userId)
    {
        if (!_cache.TryGetValue(userId, out var data))
        {
            return null;
        }

        return new UserPermissionData
        {
            Roles = data.Roles.ToFrozenSet(),
            Permissions = data.Permissions.ToFrozenSet()
        };
    }

    /// <summary>
    /// Clears and reloads all user permissions from the data store.
    /// </summary>
    public async Task ReloadAllAsync()
    {
        _logger.LogInformation("Reloading all user permissions...");
        _cache.Clear();
        await LoadAllAsync();
    }

    /// <summary>
    /// Reloads a single user's permissions from the data store.
    /// In production, this would query the database for the specific user.
    /// </summary>
    public Task ReloadUserAsync(string userId)
    {
        _logger.LogInformation("Reloading permissions for user {UserId}.", userId);

        // In production: fetch from database and update the cache.
        // For now, re-run LoadAllAsync which re-seeds everything.
        // A real implementation would do:
        //   var data = await _database.GetUserPermissionsAsync(userId);
        //   _cache.AddOrUpdate(userId, data, (_, _) => data);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes a single user from the in-memory cache.
    /// </summary>
    public void InvalidateUser(string userId)
    {
        if (_cache.TryRemove(userId, out _))
        {
            _logger.LogInformation("Invalidated permissions for user {UserId}.", userId);
        }
    }

    /// <summary>
    /// Clears the entire in-memory permission cache.
    /// </summary>
    public void InvalidateAll()
    {
        _cache.Clear();
        _logger.LogInformation("Invalidated all cached permissions.");
    }
}
