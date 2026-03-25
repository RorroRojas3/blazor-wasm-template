using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using RR.Common.Auth;
using RR.UI.Services;

namespace RR.UI.Auth;

/// <summary>
/// Scoped service that fetches the current user's permissions from the backend API
/// and caches the result for the lifetime of the browser session.
/// Inherits the pre-configured <c>RR.Api</c> HttpClient from <see cref="BaseService"/>,
/// so the Bearer token is attached automatically.
/// </summary>
public sealed class PermissionService(IHttpClientFactory httpClientFactory)
    : BaseService(httpClientFactory)
{
    private UserPermissions? _cached;
    private readonly SemaphoreSlim _semaphore = new(1, 1);

    /// <summary>
    /// Returns the current user's permissions, fetching from the API on first call
    /// and returning the cached result on subsequent calls.
    /// </summary>
    public async Task<UserPermissions> GetPermissionsAsync()
    {
        if (_cached is not null)
        {
            return _cached;
        }

        await _semaphore.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (_cached is not null)
            {
                return _cached;
            }

            _cached = await Http.GetFromJsonAsync<UserPermissions>("api/permissions")
                ?? new UserPermissions();

            return _cached;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Clears the cached permissions and re-fetches from the API.
    /// </summary>
    public async Task RefreshAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            _cached = null;
        }
        finally
        {
            _semaphore.Release();
        }

        // Re-fetch outside the lock
        await GetPermissionsAsync();
    }

    /// <summary>
    /// Clears the cached permissions without re-fetching.
    /// The next call to <see cref="GetPermissionsAsync"/> will hit the API.
    /// </summary>
    public void ClearCache()
    {
        _cached = null;
    }

    /// <summary>Shortcut: checks if the user has the specified role.</summary>
    public async Task<bool> HasRoleAsync(AppRole role)
    {
        var perms = await GetPermissionsAsync();
        return perms.HasRole(role);
    }

    /// <summary>Shortcut: checks if the user has the specified permission.</summary>
    public async Task<bool> HasPermissionAsync(AppPermission permission)
    {
        var perms = await GetPermissionsAsync();
        return perms.HasPermission(permission);
    }
}
