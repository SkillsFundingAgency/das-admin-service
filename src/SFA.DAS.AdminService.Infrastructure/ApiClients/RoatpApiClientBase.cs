using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Exceptions;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients;

public abstract class RoatpApiClientBase<L>
{
    protected readonly HttpClient _client;
    protected readonly ILogger<L> _logger;

    protected RoatpApiClientBase(HttpClient client, ILogger<L> logger)
    {
        _client = client;
        _logger = logger;
    }

    /// <summary>
    /// HTTP GET to the specified URI
    /// </summary>
    /// <typeparam name="T">The type of the object to read.</typeparam>
    /// <param name="uri">The URI to the end point you wish to interact with.</param>
    /// <returns>A Task yielding the result (of type T).</returns>
    /// <exception cref="RoatpApiClientException">Thrown if there was an unsuccessful response.</exception>
    /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
    protected async Task<T> Get<T>(string uri) where T : new()
    {
        using var response = await _client.GetAsync(new Uri(uri, UriKind.Relative));
        await LogErrorIfUnsuccessfulResponse(response);
        ThrowExceptionIfUnsuccessfulResponse(response);
        return await response.Content.ReadAsAsync<T>();
    }

    /// <summary>
    /// HTTP POST to the specified URI
    /// </summary>
    /// <param name="uri">The URI to the end point you wish to interact with.</param>
    /// <returns>The HttpStatusCode, which is the responsibility of the caller to handle.</returns>
    /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
    /// <remarks><see cref="RoatpApiClientException"/> will not be thrown - it is the responsibility of the caller to determine how an unsuccessful response is handled.</remarks>
    protected async Task<HttpStatusCode> Post<T>(string uri, T model)
    {
        var serializeObject = JsonConvert.SerializeObject(model);

        using var response = await _client.PostAsync(
            new Uri(uri, UriKind.Relative),
            new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"));
        await LogErrorIfUnsuccessfulResponse(response);
        return response.StatusCode;
    }

    /// <summary>
    /// HTTP DELETE to the specified URI
    /// </summary>
    /// <param name="uri">The URI to the end point you wish to interact with.</param>
    /// <returns>The HttpStatusCode, which is the responsibility of the caller to handle.</returns>
    /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
    protected async Task<HttpStatusCode> Delete(string uri)
    {
        using var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative));
        await LogErrorIfUnsuccessfulResponse(response);
        return response.StatusCode;
    }

    private async Task LogErrorIfUnsuccessfulResponse(HttpResponseMessage response)
    {
        if (response?.RequestMessage != null && !response.IsSuccessStatusCode)
        {
            var httpMethod = response.RequestMessage.Method.ToString();
            var statusCode = (int)response.StatusCode;
            var requestUri = response.RequestMessage.RequestUri;

            var responseContent = await response.Content.ReadAsStringAsync();
            var message = TryParseJson<ApiError>(responseContent, out var apiError) ? apiError?.Message : responseContent;

            _logger.LogError("Failed to make HTTP {HttpMethod} request on {RequestUri}, received response {StatusCode} with message: {Message}", httpMethod, requestUri, statusCode, message);
        }
    }

    private static void ThrowExceptionIfUnsuccessfulResponse(HttpResponseMessage response)
    {
        if (response?.RequestMessage != null && !response.IsSuccessStatusCode)
        {
            var httpMethod = response.RequestMessage.Method.ToString();
            var requestUri = response.RequestMessage.RequestUri;
            throw new RoatpApiClientException(response, $"Error when processing response: {httpMethod} - {requestUri}");
        }
    }

    private static bool TryParseJson<T>(string json, out T result) where T : new()
    {
        try
        {
            result = JsonConvert.DeserializeObject<T>(json);
            return true;
        }
        catch (JsonException)
        {
            // The JSON is a different type
            result = default(T);
            return false;
        }
    }
}
