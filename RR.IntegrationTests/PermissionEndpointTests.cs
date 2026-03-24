using System.Net;
using System.Net.Http.Json;
using RR.Common.DTOs;
using RR.IntegrationTests.Infrastructure;

namespace RR.IntegrationTests;

/// <summary>
/// Integration tests for the permission API endpoints.
/// </summary>
public sealed class PermissionEndpointTests(RRWebApplicationFactory factory)
    : IClassFixture<RRWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetMyPermissions_ReturnsOkWithPermissions()
    {
        var response = await _client.GetAsync("/api/permissions");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var data = await response.Content.ReadFromJsonAsync<UserPermissionData>();
        Assert.NotNull(data);
        Assert.NotEmpty(data.Roles);
        Assert.NotEmpty(data.Permissions);
    }

    [Fact]
    public async Task InvalidateUser_ReturnsNoContent()
    {
        var response = await _client.PostAsync(
            "/api/permissions/00000000-0000-0000-0000-000000000001/invalidate", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ReloadAll_ReturnsNoContent()
    {
        var response = await _client.PostAsync("/api/permissions/reload", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task ReloadUser_ReturnsNoContent()
    {
        var response = await _client.PostAsync(
            "/api/permissions/00000000-0000-0000-0000-000000000001/reload", null);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}
