using System.Net;
using System.Net.Http.Json;
using RR.Common.DTOs;
using RR.IntegrationTests.Infrastructure;

namespace RR.IntegrationTests;

/// <summary>
/// Integration tests for the reference data API endpoint.
/// </summary>
public sealed class ReferenceEndpointTests(RRWebApplicationFactory factory)
    : IClassFixture<RRWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetReferences_ReturnsOkWithReferenceData()
    {
        var response = await _client.GetAsync("/api/references");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var references = await response.Content.ReadFromJsonAsync<ReferenceData[]>();
        Assert.NotNull(references);
        Assert.NotEmpty(references);
    }

    [Fact]
    public async Task GetReferences_ReturnsEntriesWithExpectedShape()
    {
        var references = await _client.GetFromJsonAsync<ReferenceData[]>("/api/references");

        Assert.NotNull(references);

        var first = references[0];
        Assert.True(first.Id > 0);
        Assert.False(string.IsNullOrWhiteSpace(first.Code));
        Assert.False(string.IsNullOrWhiteSpace(first.DisplayName));
        Assert.False(string.IsNullOrWhiteSpace(first.Category));
    }

    [Fact]
    public async Task GetReferences_ContainsMultipleCategories()
    {
        var references = await _client.GetFromJsonAsync<ReferenceData[]>("/api/references");

        Assert.NotNull(references);

        var categories = references.Select(r => r.Category).Distinct().ToList();
        Assert.True(categories.Count > 1, "Expected reference data to contain multiple categories.");
    }
}
