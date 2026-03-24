using RR.Common.DTOs;

namespace RR.Api.Endpoints;

/// <summary>
/// Maps weather forecast API endpoints using minimal API.
/// </summary>
public static class WeatherEndpoints
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild",
        "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    /// <summary>
    /// Registers all <c>/weatherforecast</c> routes.
    /// </summary>
    public static IEndpointRouteBuilder MapWeatherEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/weatherforecast", GetWeatherForecast)
            .WithName("GetWeatherForecast")
            .RequireAuthorization()
            .WithTags("Weather");

        return app;
    }

    /// <summary>
    /// Returns a random five-day weather forecast.
    /// </summary>
    private static WeatherForecast[] GetWeatherForecast()
    {
        return Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            ))
            .ToArray();
    }
}
