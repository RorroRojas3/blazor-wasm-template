using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RR.Api.Services;

namespace RR.Api.Controllers;

/// <summary>
/// Exposes the current user's permissions and administrative cache-management endpoints.
/// All endpoints require a valid Entra ID JWT Bearer token.
/// </summary>
[ApiController]
[Authorize]
[Route("api/permissions")]
public sealed class PermissionsController(PermissionService permissionService) : ControllerBase
{
    /// <summary>
    /// Returns the authenticated user's roles and permissions.
    /// The user is identified by the <c>oid</c> (Object ID) claim from the Entra ID JWT.
    /// </summary>
    [HttpGet]
    public IActionResult GetMyPermissions()
    {
        var objectId = GetObjectId();
        if (objectId is null)
        {
            return Unauthorized("Missing 'oid' claim in token.");
        }

        var data = permissionService.GetPermissions(objectId);
        if (data is null)
        {
            // User is authenticated but has no permissions entry — return empty sets.
            return Ok(new { Roles = Array.Empty<string>(), Permissions = Array.Empty<string>() });
        }

        return Ok(data);
    }

    /// <summary>
    /// Removes a single user's permissions from the in-memory cache.
    /// </summary>
    [HttpPost("{userId}/invalidate")]
    public IActionResult InvalidateUser(string userId)
    {
        permissionService.InvalidateUser(userId);
        return NoContent();
    }

    /// <summary>
    /// Reloads all user permissions from the data store into memory.
    /// </summary>
    [HttpPost("reload")]
    public async Task<IActionResult> ReloadAll()
    {
        await permissionService.ReloadAllAsync();
        return NoContent();
    }

    /// <summary>
    /// Reloads a single user's permissions from the data store.
    /// </summary>
    [HttpPost("{userId}/reload")]
    public async Task<IActionResult> ReloadUser(string userId)
    {
        await permissionService.ReloadUserAsync(userId);
        return NoContent();
    }

    /// <summary>
    /// Extracts the Entra Object ID from the JWT claims.
    /// Microsoft Identity Web maps the "oid" claim to the full URI claim type.
    /// </summary>
    private string? GetObjectId()
    {
        return User.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? User.FindFirstValue("oid");
    }
}
