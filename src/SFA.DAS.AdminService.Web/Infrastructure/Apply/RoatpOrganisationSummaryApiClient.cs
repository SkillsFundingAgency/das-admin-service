using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Models;


namespace SFA.DAS.AdminService.Web.Infrastructure.Apply
{
    public class RoatpOrganisationSummaryApiClient: IRoatpOrganisationSummaryApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpOrganisationSummaryApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;
        private const string RoutePath = "organisation";

        public RoatpOrganisationSummaryApiClient(string baseUri, ILogger<RoatpOrganisationSummaryApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }
        public async Task<string> GetTypeOfOrganisation(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving type of organisation from applicationId [{applicationId}]");
            return await GetString($"{RoutePath}/TypeOfOrganisation/{applicationId}");
        }

        public async Task<string> GetCompanyNumber(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving company number from applicationId [{applicationId}]");
            return await GetString($"{RoutePath}/CompanyNumber/{applicationId}");
        }

        public async Task<string> GetCharityNumber(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving charity number from applicationId [{applicationId}]");
            return await GetString($"{RoutePath}/CharityNumber/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetDirectorsFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of directors submitted in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/DirectorData/Submitted/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetDirectorsFromCompaniesHouse(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of directors from companies house in ApplyData from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/DirectorData/CompaniesHouse/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetPscsFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of pscs in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/PscData/Submitted/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetPscsFromCompaniesHouse(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of pscs from companies house in ApplyData from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/PscData/CompaniesHouse/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetTrusteesFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of trustees in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/TrusteeData/Submitted/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetTrusteesFromCharityCommission(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of trustees from charity commission in ApplyData from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/TrusteeData/CharityCommission/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetWhosInControlFromSubmitted(Guid applicationId)
        {
            _logger.LogInformation($"Retrieving list of Whos in control in Qna QuestionTags from applicationId [{applicationId}]");
            return await Get<List<PersonInControl>>($"{RoutePath}/WhosInControlData/Submitted/{applicationId}");
        }

        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        private async Task<string> GetString(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
