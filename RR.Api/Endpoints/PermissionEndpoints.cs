using System.Security.Claims;
using RR.Service;

namespace RR.Api.Endpoints;

/// <summary>
/// Maps permission-related API endpoints using minimal API.
/// </summary>
public static class PermissionEndpoints
{
    /// <summary>
    /// Registers all <c>/api/permissions</c> routes.
    /// </summary>
    public static IEndpointRouteBuilder MapPermissionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/permissions")
            .RequireAuthorization()
            .WithTags("Permissions");

        group.MapGet("/", GetMyPermissions)
            .WithName("GetMyPermissions");

        group.MapPost("/{userId}/invalidate", InvalidateUser)
            .WithName("InvalidateUser");

        group.MapPost("/reload", ReloadAll)
            .WithName("ReloadAll");

        group.MapPost("/{userId}/reload", ReloadUser)
            .WithName("ReloadUser");

        return app;
    }

    /// <summary>
    /// Returns the authenticated user's roles and permissions.
    /// </summary>
    private static IResult GetMyPermissions(ClaimsPrincipal user, IPermissionService permissionService)
    {
        var objectId = GetObjectId(user);
        if (objectId is null)
        {
            return Results.Unauthorized();
        }

        var data = permissionService.GetPermissions(objectId);
        if (data is null)
        {
            return Results.Ok(new { Roles = Array.Empty<string>(), Permissions = Array.Empty<string>() });
        }

        return Results.Ok(data);
    }

    /// <summary>
    /// Removes a single user's permissions from the in-memory cache.
    /// </summary>
    private static IResult InvalidateUser(string userId, IPermissionService permissionService)
    {
        permissionService.InvalidateUser(userId);
        return Results.NoContent();
    }

    /// <summary>
    /// Reloads all user permissions from the data store into memory.
    /// </summary>
    private static async Task<IResult> ReloadAll(IPermissionService permissionService)
    {
        await permissionService.ReloadAllAsync();
        return Results.NoContent();
    }

    /// <summary>
    /// Reloads a single user's permissions from the data store.
    /// </summary>
    private static async Task<IResult> ReloadUser(string userId, IPermissionService permissionService)
    {
        await permissionService.ReloadUserAsync(userId);
        return Results.NoContent();
    }

    /// <summary>
    /// Extracts the Entra Object ID from the JWT claims.
    /// </summary>
    private static string? GetObjectId(ClaimsPrincipal user)
    {
        return user.FindFirstValue("http://schemas.microsoft.com/identity/claims/objectidentifier")
            ?? user.FindFirstValue("oid");
    }
}
