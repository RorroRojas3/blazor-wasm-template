namespace RR.Entity;

/// <summary>
/// Placeholder entity representing a sample database record.
/// </summary>
public sealed class SampleEntity
{
    /// <summary>
    /// Unique identifier for the sample record.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name of the sample record.
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Timestamp when the record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
