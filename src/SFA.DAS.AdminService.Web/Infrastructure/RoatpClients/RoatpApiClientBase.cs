using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public abstract class RoatpApiClientBase<CB>
    {
        protected readonly HttpClient _client;
        protected readonly ILogger<CB> _logger;
        protected readonly IRoatpTokenService _tokenService;

        public RoatpApiClientBase(string baseUri, ILogger<CB> logger, IRoatpTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }

        protected async Task<T> Get<T>(string uri) where T : new()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            try
            {
                using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<T>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{HttpMethod.Get}: Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<string> Get(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            try
            {
                using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsStringAsync();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{HttpMethod.Get}: Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<HttpResponseMessage> GetResponse(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            try
            {
                var response = await _client.GetAsync(new Uri(uri, UriKind.Relative));

                await LogErrorIfUnsuccessfulResponse(response);
                return response;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{HttpMethod.Get}: Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) 
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{HttpMethod.Post}: Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<U> Post<T, U>(string uri, T model) where U : new()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<U>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{HttpMethod.Post}: Error when processing request to: {uri}");
                throw;
            }
        }

        protected async Task<U> Put<T, U>(string uri, T model) where U : new()
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                    new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    await LogErrorIfUnsuccessfulResponse(response);
                    return await response.Content.ReadAsAsync<U>();
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"{HttpMethod.Put}: Error when processing request to: {uri}");
                throw;
            }
        }

        private async Task LogErrorIfUnsuccessfulResponse(HttpResponseMessage response)
        {
            if (response?.RequestMessage != null && !response.IsSuccessStatusCode)
            {
                var httpMethod = response.RequestMessage.Method;
                var statusCode = (int)response.StatusCode;
                var requestUri = response.RequestMessage.RequestUri;

                var responseContent = await response.Content.ReadAsStringAsync();
                var errorMessage = TryParseJson<ApiError>(responseContent, out var apiError) ? apiError?.Message : responseContent;

                _logger.LogError($"{httpMethod} || StatusCode: {statusCode} || RequestUri: {requestUri} || ErrorMessage: {errorMessage}");
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
