using RR.Common.DTOs;

namespace RR.Api.Endpoints;

/// <summary>
/// Maps reference data API endpoints using minimal API.
/// </summary>
public static class ReferenceEndpoints
{
    /// <summary>
    /// Registers all <c>/api/references</c> routes.
    /// </summary>
    public static IEndpointRouteBuilder MapReferenceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/references")
            .RequireAuthorization()
            .WithTags("References");

        group.MapGet("/", GetReferences)
            .WithName("GetReferences");

        return app;
    }

    /// <summary>
    /// Returns a static list of reference data entries used for dropdowns and lookups.
    /// </summary>
    private static ReferenceData[] GetReferences()
    {
        return
        [
            new(1, "US", "United States", "Country"),
            new(2, "CA", "Canada", "Country"),
            new(3, "MX", "Mexico", "Country"),
            new(4, "GB", "United Kingdom", "Country"),
            new(5, "ACTIVE", "Active", "Status"),
            new(6, "INACTIVE", "Inactive", "Status"),
            new(7, "PENDING", "Pending", "Status"),
            new(8, "GENERAL", "General", "Category"),
            new(9, "FINANCE", "Finance", "Category"),
            new(10, "ENGINEERING", "Engineering", "Category")
        ];
    }
}
