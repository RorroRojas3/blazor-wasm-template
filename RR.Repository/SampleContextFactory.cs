using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace RR.Repository;

/// <summary>
/// Design-time factory for creating <see cref="SampleContext"/> instances.
/// Used by EF Core CLI tools for migrations.
/// </summary>
public sealed class SampleContextFactory : IDesignTimeDbContextFactory<SampleContext>
{
    /// <inheritdoc />
    public SampleContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SampleContext>();
        optionsBuilder.UseSqlServer("Server=.;Database=SampleDb;Trusted_Connection=True;TrustServerCertificate=True;");

        return new SampleContext(optionsBuilder.Options);
    }
}
