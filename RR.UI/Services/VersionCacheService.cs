using Microsoft.JSInterop;

namespace RR.UI.Services;

/// <summary>
/// Compares the current app version (from <c>appsettings.json</c>) against the
/// version stored in <c>localStorage</c>. When a mismatch is detected — i.e. a
/// new release has been deployed — all non-MSAL <c>localStorage</c> entries are
/// cleared. Intended to run once at application startup.
/// </summary>
public sealed class VersionCacheService(
    IConfiguration configuration,
    IJSRuntime jsRuntime) : IAsyncDisposable
{
    private IJSObjectReference? _module;

    /// <summary>
    /// Checks the stored version against the configured app version.
    /// If they differ, clears non-MSAL localStorage keys and persists the new version.
    /// </summary>
    /// <returns><see langword="true"/> if a cache bust occurred.</returns>
    public async Task<bool> CheckAndBustCacheAsync()
    {
        var currentVersion = configuration["AppVersion"];
        if (string.IsNullOrWhiteSpace(currentVersion))
        {
            return false;
        }

        _module ??= await jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./js/versionCache.js");

        var storedVersion = await _module.InvokeAsync<string?>("getStoredVersion");

        if (string.Equals(storedVersion, currentVersion, StringComparison.Ordinal))
        {
            return false;
        }

        await _module.InvokeVoidAsync("setStoredVersion", currentVersion);

        return true;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_module is not null)
        {
            await _module.DisposeAsync();
        }
    }
}
