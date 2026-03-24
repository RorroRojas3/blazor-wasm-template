using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace RR.IntegrationTests.Infrastructure;

/// <summary>
/// Authentication handler that always succeeds with a configurable set of claims.
/// Used to bypass Entra ID authentication in integration tests.
/// </summary>
public sealed class TestAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    /// <summary>
    /// The authentication scheme name used in tests.
    /// </summary>
    public const string SchemeName = "TestScheme";

    /// <summary>
    /// Default test user Object ID matching the admin seed data user.
    /// </summary>
    public const string DefaultObjectId = "00000000-0000-0000-0000-000000000001";

    /// <inheritdoc />
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", DefaultObjectId),
            new Claim(ClaimTypes.Name, "Test User"),
            new Claim(ClaimTypes.NameIdentifier, DefaultObjectId),
            new Claim("oid", DefaultObjectId)
        };

        var identity = new ClaimsIdentity(claims, SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
