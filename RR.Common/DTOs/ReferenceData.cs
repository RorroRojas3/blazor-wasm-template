namespace RR.Common.DTOs;

/// <summary>
/// Represents a single reference data entry used for dropdowns, lookups, and other UI elements.
/// </summary>
/// <param name="Id">Unique identifier for the reference entry.</param>
/// <param name="Code">Short machine-friendly code (e.g., "US", "ACTIVE").</param>
/// <param name="DisplayName">Human-readable display name.</param>
/// <param name="Category">Grouping category for the reference entry (e.g., "Country", "Status").</param>
public sealed record ReferenceData(int Id, string Code, string DisplayName, string Category);
