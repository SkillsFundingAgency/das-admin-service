﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AdminService.Infrastructure.ApiClients.Exceptions;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients
{
    public abstract class RoatpApiClientBase<L>
    {
        protected readonly HttpClient _client;
        protected readonly ILogger<L> _logger;

        public RoatpApiClientBase(HttpClient client, ILogger<L> logger)
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
            try
            {
                using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    ThrowExceptionIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<T>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Get} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP GET to the specified URI
        /// </summary>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>A Task yielding the result (as a string).</returns>
        /// <exception cref="RoatpApiClientException">Thrown if there was an unsuccessful response.</exception>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<string> Get(string uri)
        {
            try
            {
                using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    ThrowExceptionIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Get} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP GET to the specified URI
        /// </summary>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>The HttpResponseMessage, which is the responsibility of the caller to handle.</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        /// <remarks><see cref="RoatpApiClientException"/> will not be thrown - it is the responsibility of the caller to determine how an unsuccessful response is handled.</remarks>
        protected async Task<HttpResponseMessage> GetResponse(string uri)
        {
            try
            {
                var response = await _client.GetAsync(new Uri(uri, UriKind.Relative));

                await LogErrorIfUnsuccessfulResponse(response);
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Get} - {uri}");
                throw;
            }
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

            try
            {
                using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) 
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return response.StatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Post} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP POST to the specified URI
        /// </summary>
        /// <typeparam name="T">The type of the object to POST.</typeparam>
        /// <typeparam name="U">The type of the object to read.</typeparam>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>A Task yielding the result (of type U).</returns>
        /// <exception cref="RoatpApiClientException">Thrown if there was an unsuccessful response.</exception>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<U> Post<T, U>(string uri, T model) where U : new()
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    ThrowExceptionIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<U>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Post} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP PUT to the specified URI
        /// </summary>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>The HttpStatusCode, which is the responsibility of the caller to handle.</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        /// <remarks><see cref="RoatpApiClientException"/> will not be thrown - it is the responsibility of the caller to determine how an unsuccessful response is handled.</remarks>
        protected async Task<HttpStatusCode> Put<T>(string uri, T model)
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return response.StatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Put} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP PUT to the specified URI
        /// </summary>
        /// <typeparam name="T">The type of the object to PUT.</typeparam>
        /// <typeparam name="U">The type of the object to read.</typeparam>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>A Task yielding the result (of type U).</returns>
        /// <exception cref="RoatpApiClientException">Thrown if there was an unsuccessful response.</exception>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<U> Put<T, U>(string uri, T model) where U : new()
        {
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    ThrowExceptionIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<U>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Put} - {uri}");
                throw;
            }
        }

        /// <summary>
        /// HTTP DELETE to the specified URI
        /// </summary>
        /// <param name="uri">The URI to the end point you wish to interact with.</param>
        /// <returns>The HttpStatusCode, which is the responsibility of the caller to handle.</returns>
        /// <exception cref="HttpRequestException">Thrown if something unexpected occurred when sending the request.</exception>
        protected async Task<HttpStatusCode> Delete(string uri)
        {
            try
            {
                using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return response.StatusCode;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error when processing request: {HttpMethod.Delete} - {uri}");
                throw;
            }
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

                _logger.LogError($"HTTP {statusCode} || {httpMethod}: {requestUri} || Message: {message}");
            }
        }

        private void ThrowExceptionIfUnsuccessfulResponse(HttpResponseMessage response)
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
}
