using System.Net;
using System.Net.Http.Json;
using RR.Common.DTOs;
using RR.IntegrationTests.Infrastructure;

namespace RR.IntegrationTests;

/// <summary>
/// Integration tests for the weather forecast API endpoint.
/// </summary>
public sealed class WeatherEndpointTests(RRWebApplicationFactory factory)
    : IClassFixture<RRWebApplicationFactory>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task GetWeatherForecast_ReturnsOkWithForecasts()
    {
        var response = await _client.GetAsync("/weatherforecast");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var forecasts = await response.Content.ReadFromJsonAsync<WeatherForecast[]>();
        Assert.NotNull(forecasts);
        Assert.Equal(5, forecasts.Length);
    }
}
