using Microsoft.EntityFrameworkCore;
using RR.Entity;

namespace RR.Repository;

/// <summary>
/// Entity Framework Core database context for the application.
/// Configured to use Azure SQL Database.
/// </summary>
public sealed class SampleContext(DbContextOptions<SampleContext> options) : DbContext(options)
{
    /// <summary>
    /// Sample entities stored in the database.
    /// </summary>
    public DbSet<SampleEntity> Samples => Set<SampleEntity>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SampleEntity>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
