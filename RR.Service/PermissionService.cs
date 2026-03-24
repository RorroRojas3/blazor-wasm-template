using System.Collections.Concurrent;
using System.Collections.Frozen;
using Microsoft.Extensions.Logging;
using RR.Common.Auth;
using RR.Common.DTOs;
using RR.Model;

namespace RR.Service;

/// <summary>
/// Singleton service that stores all user permissions in memory.
/// Data is loaded at application startup and served from a <see cref="ConcurrentDictionary{TKey,TValue}"/>
/// so that lookups are synchronous and thread-safe.
/// </summary>
public sealed class PermissionService(ILogger<PermissionService> logger) : IPermissionService
{
    private readonly ConcurrentDictionary<string, CachedUserPermissions> _cache = new(StringComparer.OrdinalIgnoreCase);
    private readonly ILogger<PermissionService> _logger = logger;

    /// <inheritdoc />
    public Task LoadAllAsync()
    {
        _logger.LogInformation("Loading all user permissions into memory...");

        // ──────────────────────────────────────────────────────────────
        // SEED DATA — replace with real database / external store calls.
        // Keys are Entra Object IDs (the "oid" claim from the JWT).
        // ──────────────────────────────────────────────────────────────

        var seedData = new Dictionary<string, CachedUserPermissions>(StringComparer.OrdinalIgnoreCase)
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

    /// <inheritdoc />
    public UserPermissionData? GetPermissions(string userId)
    {
        if (!_cache.TryGetValue(userId, out var data))
        {
            return null;
        }

        return new UserPermissionData
        {
            Roles = data.Roles.ToFrozenSet().ToHashSet(),
            Permissions = data.Permissions.ToFrozenSet().ToHashSet()
        };
    }

    /// <inheritdoc />
    public async Task ReloadAllAsync()
    {
        _logger.LogInformation("Reloading all user permissions...");
        _cache.Clear();
        await LoadAllAsync();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void InvalidateUser(string userId)
    {
        if (_cache.TryRemove(userId, out _))
        {
            _logger.LogInformation("Invalidated permissions for user {UserId}.", userId);
        }
    }

    /// <inheritdoc />
    public void InvalidateAll()
    {
        _cache.Clear();
        _logger.LogInformation("Invalidated all cached permissions.");
    }
}
