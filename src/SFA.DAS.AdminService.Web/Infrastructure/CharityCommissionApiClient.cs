using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class CharityCommissionApiClient:ICharityCommissionApiClient
    {
        private ILogger<CharityCommissionApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;
        private static  HttpClient _httpClient = new HttpClient();

        public CharityCommissionApiClient(string baseUri,
            ILogger<CharityCommissionApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _tokenService = tokenService;
            _logger = logger;
 
            _httpClient = new HttpClient { BaseAddress = new Uri(baseUri) };
            
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
        }

        public async Task<ApiResponse<Charity>> GetCharityDetails(int charityNumber)
        {
            var responseMessage =
                await _httpClient.GetAsync($"/charity-commission-lookup?charityNumber={charityNumber}");

            if (responseMessage.StatusCode == HttpStatusCode.InternalServerError ||
                responseMessage.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                return new ApiResponse<Charity> { Success = false };
            }

            return new ApiResponse<Charity>
            {
                Success = true,
                Response = await responseMessage.Content.ReadAsAsync<Charity>()
            };
        }
    }
}
