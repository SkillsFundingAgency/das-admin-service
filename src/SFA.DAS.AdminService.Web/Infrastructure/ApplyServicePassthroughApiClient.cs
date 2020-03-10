using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class ApplyServicePassthroughApiClient: IApplyServicePassthroughApiClient
    {
        private readonly HttpClient _client;
        private readonly IRoatpApplyTokenService _tokenService;

        public ApplyServicePassthroughApiClient(string baseUri, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _tokenService = tokenService;
        }

        public async Task<CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber)
        {
            return await Get<CompaniesHouseSummary>($"/companies-house-lookup?companyNumber={companiesHouseNumber}");
        }

        public async Task<Charity> GetCharityDetails(string charityNumber)
        {
            return await Get<Charity>($"/charity-commission-lookup?charityNumber={charityNumber}");
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
    }
}
