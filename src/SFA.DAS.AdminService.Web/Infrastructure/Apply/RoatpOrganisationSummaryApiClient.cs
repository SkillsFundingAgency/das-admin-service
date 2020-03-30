using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.AdminService.Web.Models;


namespace SFA.DAS.AdminService.Web.Infrastructure.Apply
{
    public class RoatpOrganisationSummaryApiClient: IRoatpOrganisationSummaryApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpOrganisationSummaryApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;

        public RoatpOrganisationSummaryApiClient(string baseUri, ILogger<RoatpOrganisationSummaryApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }
        public async Task<string> GetTypeOfOrganisation(Guid applicationId)
        {
            return await GetString($"organisation/TypeOfOrganisation/{applicationId}");
        }

        public async Task<TabularData> GetDirectors(Guid applicationId)
        {
            var directors = await Get<TabularData>($"organisation/DirectorData/{applicationId}");
            return directors;
        }

        public async Task<TabularData> GetPersonsWithSignificantControl(Guid applicationId)
        {
            var pscs = await Get<TabularData>($"organisation/PscData/{applicationId}");
            return pscs;
        }

        public async Task<TabularData> GetTrustees(Guid applicationId)
        {
            var trustees = await Get<TabularData>($"organisation/TrusteeData/{applicationId}");
            return trustees;
        }

        public async Task<TabularData> GetPeopleInControl(Guid applicationId)
        {
            var peopleInControl = await Get<TabularData>($"organisation/PeopleInControlData/{applicationId}");
            return peopleInControl;
        }

        public async Task<TabularData> GetPartners(Guid applicationId)
        {
            var partners = await Get<TabularData>($"organisation/PartnersData/{applicationId}");
            return partners;
        }

        public async Task<string> GetSoleTraderDob(Guid applicationId)
        {
            return await GetString($"organisation/SoleTraderDob/{applicationId}");
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
