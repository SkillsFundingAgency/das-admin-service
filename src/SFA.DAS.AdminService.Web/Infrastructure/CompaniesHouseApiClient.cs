using AutoMapper;
using global::System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class CompaniesHouseApiClient:ICompaniesHouseApiClient
    {
        private ILogger<CompaniesHouseApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;
        private static  HttpClient _httpClient = new HttpClient();
        private readonly IMapper _mapper;

        public CompaniesHouseApiClient(string baseUri,
            ILogger<CompaniesHouseApiClient> logger, IRoatpApplyTokenService tokenService, IMapper mapper)
        {
            _tokenService = tokenService;
            _logger = logger;
            _mapper = mapper;

            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
        }

        public async Task<CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber)
        {
            var requestMessage =
                await _httpClient.GetAsync($"companies-house-lookup?companyNumber={companiesHouseNumber}");

            if (requestMessage.IsSuccessStatusCode)
            {
                var companyDetails = await requestMessage.Content.ReadAsAsync<CompaniesHouseSummary>();

                return _mapper.Map<CompaniesHouseSummary>(companyDetails);
            }

            if (requestMessage.StatusCode == HttpStatusCode.ServiceUnavailable || requestMessage.StatusCode == HttpStatusCode.InternalServerError)
            {
                return new CompaniesHouseSummary { Status = "service_unavailable" };
            }

            return new CompaniesHouseSummary { Status = "not_found" };
        }
    }
}
