using System.Net.Http.Json;
using RR.Common.DTOs;
using RR.UI.Services;

namespace RR.UI.Stores;

/// <summary>
/// Scoped store that fetches and caches reference data from the backend API.
/// Components subscribe to <see cref="OnChange"/> to re-render when the data updates.
/// <example>
/// <code>
/// @inject ReferenceStore ReferenceStore
/// @implements IDisposable
///
/// @code {
///     protected override async Task OnInitializedAsync()
///     {
///         ReferenceStore.OnChange += StateHasChanged;
///         await ReferenceStore.LoadAsync();
///     }
///
///     public void Dispose() => ReferenceStore.OnChange -= StateHasChanged;
/// }
/// </code>
/// </example>
/// </summary>
public sealed class ReferenceStore(IHttpClientFactory httpClientFactory)
    : BaseService(httpClientFactory)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private List<ReferenceData> _references = [];

    /// <summary>
    /// Event raised whenever the reference data changes.
    /// Components should subscribe to this and call <c>StateHasChanged()</c>.
    /// </summary>
    public event Action? OnChange;

    /// <summary>
    /// The cached reference data. Empty until <see cref="LoadAsync"/> completes.
    /// </summary>
    public IReadOnlyList<ReferenceData> References => _references;

    /// <summary>
    /// Indicates whether the store has successfully loaded data at least once.
    /// </summary>
    public bool IsLoaded { get; private set; }

    /// <summary>
    /// Fetches reference data from the API if not already loaded.
    /// </summary>
    public async Task LoadAsync()
    {
        if (IsLoaded)
        {
            return;
        }

        await _semaphore.WaitAsync();
        try
        {
            // Double-check after acquiring lock
            if (IsLoaded)
            {
                return;
            }

            var data = await Http.GetFromJsonAsync<List<ReferenceData>>("api/references");
            _references = data ?? [];
            IsLoaded = true;
            NotifyStateChanged();
        }
        finally
        {
            _semaphore.Release();
        }
    }

    /// <summary>
    /// Clears the cache and re-fetches reference data from the API.
    /// </summary>
    public async Task RefreshAsync()
    {
        await _semaphore.WaitAsync();
        try
        {
            IsLoaded = false;
            _references = [];
        }
        finally
        {
            _semaphore.Release();
        }

        await LoadAsync();
    }

    /// <summary>
    /// Invokes the <see cref="OnChange"/> event to notify subscribers.
    /// </summary>
    private void NotifyStateChanged() => OnChange?.Invoke();
}
