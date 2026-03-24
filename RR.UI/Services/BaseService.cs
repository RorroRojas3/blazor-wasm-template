namespace RR.UI.Services;

/// <summary>
/// Base class for services that communicate with the RR.Api backend.
/// Creates the named <c>RR.Api</c> <see cref="HttpClient"/> by default,
/// which includes the Bearer token via <c>AuthorizationMessageHandler</c>.
/// </summary>
public abstract class BaseService(IHttpClientFactory httpClientFactory)
{
    /// <summary>The pre-configured HttpClient for the RR.Api backend.</summary>
    protected HttpClient Http { get; } = httpClientFactory.CreateClient("RR.Api");
}
