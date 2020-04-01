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
            return await GetString($"{RoutePath}/TypeOfOrganisation/{applicationId}");
        }

        public async Task<List<PersonInControl>> GetDirectorsFromSubmitted(Guid applicationId)
        {
            var directors = await Get<List<PersonInControl>>($"{RoutePath}/DirectorData/Submitted/{applicationId}");
            return directors;
        }

        public async Task<List<PersonInControl>> GetDirectorsFromCompaniesHouse(Guid applicationId)
        {
            var directors = await Get<List<PersonInControl>>($"{RoutePath}/DirectorData/CompaniesHouse/{applicationId}");
            return directors;
        }

        public async Task<List<PersonInControl>> GetPscsFromSubmitted(Guid applicationId)
        {
            var pscs = await Get<List<PersonInControl>>($"{RoutePath}/PscData/Submitted/{applicationId}");
            return pscs;
        }

        public async Task<List<PersonInControl>> GetPscsFromCompaniesHouse(Guid applicationId)
        {
            var pscs = await Get<List<PersonInControl>>($"{RoutePath}/PscData/CompaniesHouse/{applicationId}");
            return pscs;
        }

        public async Task<List<PersonInControl>> GetTrusteesFromSubmitted(Guid applicationId)
        {
            var pscs = await Get<List<PersonInControl>>($"{RoutePath}/TrusteeData/Submitted/{applicationId}");
            return pscs;
        }

        public async Task<List<PersonInControl>> GetTrusteesFromCharityCommission(Guid applicationId)
        {
            var pscs = await Get<List<PersonInControl>>($"{RoutePath}/TrusteeData/CharityCommission/{applicationId}");
            return pscs;
        }

        public async Task<List<PersonInControl>> GetWhosInControlFromSubmitted(Guid applicationId)
        {
            var whosInControl = await Get<List<PersonInControl>>($"{RoutePath}/WhosInControlData/Submitted/{applicationId}");
            return whosInControl;
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
