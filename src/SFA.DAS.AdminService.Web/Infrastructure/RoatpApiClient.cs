namespace SFA.DAS.AdminService.Web.Infrastructure
{
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
    using SFA.DAS.AssessorService.Api.Types.Models.Roatp;
    using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Threading.Tasks;

    public class RoatpApiClient : IRoatpApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApiClient> _logger;
        private readonly IRoatpTokenService _tokenService;
        private readonly string _baseUrl;

        public RoatpApiClient(HttpClient client, ILogger<RoatpApiClient> logger, IRoatpTokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
            _baseUrl = _client.BaseAddress.ToString();
        }

        public RoatpApiClient(string baseUri, ILogger<RoatpApiClient> logger, IRoatpTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
            _baseUrl = _client.BaseAddress.ToString();
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetAuditHistory()
        {
            var url = $"/api/v1/download/audit";
            _logger.LogInformation($"Retrieving RoATP register audit history data from {_baseUrl}{url}");
            return await Get<IEnumerable<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetCompleteRegister()
        {
            var url = $"/api/v1/download/complete";
            _logger.LogInformation($"Retrieving RoATP complete register data from {_baseUrl}{url}");
            return await Get<IEnumerable<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<IDictionary<string, object>>> GetRoatpSummary()
        {
            var url = $"/api/v1/download/roatp-summary";
            _logger.LogInformation($"Retrieving RoATP summary data from {_baseUrl}{url}");
            return await Get<IEnumerable<IDictionary<string, object>>>(url);
        }

        public async Task<IEnumerable<OrganisationType>> GetOrganisationTypes(int? providerTypeId)
        {
            return await Get<IEnumerable<OrganisationType>>($"/api/v1/lookupData/organisationTypes?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<ProviderType>> GetProviderTypes()
        {
            return await Get<IEnumerable<ProviderType>>($"/api/v1/lookupData/providerTypes");
        }

        public async Task<IEnumerable<OrganisationStatus>> GetOrganisationStatuses(int? providerTypeId)
        {
            return await Get<IEnumerable<OrganisationStatus>>($"/api/v1/lookupData/organisationStatuses?providerTypeId={providerTypeId}");
        }

        public async Task<IEnumerable<RemovedReason>> GetRemovedReasons()
        {
            return await Get<IEnumerable<RemovedReason>>($"/api/v1/lookupData/removedReasons");
        }

        public async Task<bool> CreateOrganisation(CreateRoatpOrganisationRequest organisationRequest)
        {
            var result = await Post($"/api/v1/organisation/create", organisationRequest);
            return result is HttpStatusCode.OK;
        }

        public async Task<DuplicateCheckResponse> DuplicateUKPRNCheck(Guid organisationId, long ukprn)
        {
            return await Get<DuplicateCheckResponse>($"/api/v1/duplicateCheck/ukprn?ukprn={ukprn}&organisationId={organisationId}");
        }

        public async Task<DuplicateCheckResponse> DuplicateCompanyNumberCheck(Guid organisationId, string companyNumber)
        {
            return await Get<DuplicateCheckResponse>($"/api/v1/duplicateCheck/companyNumber?companyNumber={companyNumber}&organisationId={organisationId}");
        }

        public async Task<DuplicateCheckResponse> DuplicateCharityNumberCheck(Guid organisationId, string charityNumber)
        {
            return await Get<DuplicateCheckResponse>($"/api/v1/duplicateCheck/charityNumber?charityNumber={charityNumber}&organisationId={organisationId}");
        }

        public async Task<OrganisationSearchResults> Search(string searchTerm)
        {
            return await Get<OrganisationSearchResults>($"/api/v1/search?searchTerm={searchTerm}");
        }

        public async Task<bool> UpdateOrganisationLegalName(UpdateOrganisationLegalNameRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/legalName", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationStatus(UpdateOrganisationStatusRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/status", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationType(UpdateOrganisationTypeRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/type", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationTradingName(UpdateOrganisationTradingNameRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/tradingName", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationParentCompanyGuarantee(UpdateOrganisationParentCompanyGuaranteeRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/parentCompanyGuarantee", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationFinancialTrackRecord(UpdateOrganisationFinancialTrackRecordRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/financialTrackRecord", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationProviderType(UpdateOrganisationProviderTypeRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/providerType", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationUkprn(UpdateOrganisationUkprnRequest request)
        {
            var result = await Put($"{_baseUrl}/api/v1/updateOrganisation/ukprn", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationCompanyNumber(UpdateOrganisationCompanyNumberRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/companyNumber", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateOrganisationCharityNumber(UpdateOrganisationCharityNumberRequest request)
        {
            var result = await Put<UpdateOrganisationCharityNumberRequest>($"/api/v1/updateOrganisation/charityNumber", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<bool> UpdateApplicationDeterminedDate(UpdateOrganisationApplicationDeterminedDateRequest request)
        {
            var result = await Put($"/api/v1/updateOrganisation/applicationDeterminedDate", request);
            return result is HttpStatusCode.OK;
        }

        public async Task<IEnumerable<ProviderDetails>> GetUkrlpProviderDetails(string ukprn)
        {
            var res = await Get<UkprnLookupResponse>($"/api/v1/ukrlp/lookup/{ukprn}");

            return res.Results;
        }

        private async Task<HttpStatusCode> Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    var actualResponse = string.Empty;
                    try
                    {
                        actualResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // safe to ignore any errors
                    }
                    _logger.LogError($"POST: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                }

                return response.StatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"POST: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

        private async Task<HttpStatusCode> Put<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                var response = await _client.PutAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"));
                if (!response.IsSuccessStatusCode)
                {
                    var actualResponse = string.Empty;
                    try
                    {
                        actualResponse = await response.Content.ReadAsStringAsync();
                    }
                    catch
                    {
                        // safe to ignore any errors
                    }
                    _logger.LogError($"PUT: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                }

                return response.StatusCode;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"PUT: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            try
            {
                using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<T>();
                    }
                    catch (Exception ex)
                    {
                        var actualResponse = string.Empty;
                        try
                        {
                            actualResponse = await response.Content.ReadAsStringAsync();
                        }
                        catch
                        {
                            // safe to ignore any errors
                        }
                        _logger.LogError(ex, $"GET: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                        throw;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"GET: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            try
            {
                using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative), new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
                {
                    try
                    {
                        return await response.Content.ReadAsAsync<U>();
                    }
                    catch (Exception ex)
                    {
                        var actualResponse = string.Empty;
                        try
                        {
                            actualResponse = await response.Content.ReadAsStringAsync();
                        }
                        catch
                        {
                            // safe to ignore any errors
                        }
                        _logger.LogError(ex, $"POST: HTTP {(int)response.StatusCode} Error getting response from: {uri} - ActualResponse: {actualResponse}");
                        throw;
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"POST: HTTP Error when processing request to: {uri}");
                throw;
            }
        }

    }
}
