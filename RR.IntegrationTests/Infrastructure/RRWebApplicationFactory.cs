using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RR.Repository;
using Testcontainers.MsSql;

namespace RR.IntegrationTests.Infrastructure;

/// <summary>
/// Custom <see cref="WebApplicationFactory{TEntryPoint}"/> that replaces the database
/// with a Testcontainers MSSQL instance and bypasses authentication.
/// </summary>
public sealed class RRWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder()
        .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
        .Build();

    /// <inheritdoc />
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing SampleContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<SampleContext>));

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }

            // Register SampleContext with the Testcontainers connection string
            services.AddDbContext<SampleContext>(options =>
                options.UseSqlServer(_msSqlContainer.GetConnectionString()));

            // Replace authentication with the test handler
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName, _ => { });

            // Ensure database is created with schema
            using var scope = services.BuildServiceProvider().CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SampleContext>();
            context.Database.EnsureCreated();
        });
    }

    /// <inheritdoc />
    public async Task InitializeAsync()
    {
        await _msSqlContainer.StartAsync();
    }

    /// <inheritdoc />
    async Task IAsyncLifetime.DisposeAsync()
    {
        await _msSqlContainer.DisposeAsync();
    }
}
