using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class CompaniesHouseApiClient : ICompaniesHouseApiClient
    {
        private ILogger<CompaniesHouseApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;
        private static  HttpClient _httpClient = new HttpClient();
        
        public CompaniesHouseApiClient(string baseUri,
            ILogger<CompaniesHouseApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _tokenService = tokenService;
            _logger = logger;

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
        }

        public async Task<AssessorService.Domain.Entities.CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber)
        {
            var requestMessage =
                await _httpClient.GetAsync($"companies-house-lookup?companyNumber={companiesHouseNumber}");

            if (requestMessage.IsSuccessStatusCode)
            {
                var companyDetails = await requestMessage.Content.ReadAsAsync<AssessorService.Domain.Entities.CompaniesHouseSummary>();
                return companyDetails;
            }

            if (requestMessage.StatusCode == HttpStatusCode.ServiceUnavailable || requestMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                return new AssessorService.Domain.Entities.CompaniesHouseSummary { Status = "service_unavailable" };
            }

            return new AssessorService.Domain.Entities.CompaniesHouseSummary { Status = "not_found" };
        }
    }
}
