namespace RR.Common.DTOs;

/// <summary>
/// Represents a weather forecast entry with date, temperature, and summary.
/// </summary>
public sealed record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Temperature converted to Fahrenheit.
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
