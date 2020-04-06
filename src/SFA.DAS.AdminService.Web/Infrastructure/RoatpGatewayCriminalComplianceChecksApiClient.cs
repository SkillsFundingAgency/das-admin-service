using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpGatewayCriminalComplianceChecksApiClient : IRoatpGatewayCriminalComplianceChecksApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpGatewayCriminalComplianceChecksApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;

        public RoatpGatewayCriminalComplianceChecksApiClient(string baseUri, ILogger<RoatpGatewayCriminalComplianceChecksApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<CriminalComplianceCheckDetails> GetCriminalComplianceQuestionDetails(Guid applicationId, string gatewayPageId)
        {
            return await Get<CriminalComplianceCheckDetails>($"/Gateway/{applicationId}/CriminalCompliance/{gatewayPageId}");
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
