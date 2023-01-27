using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpOrganisationApiClient : IRoatpOrganisationApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpOrganisationApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;

        public RoatpOrganisationApiClient(string baseUri, ILogger<RoatpOrganisationApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task<Organisation> GetOrganisation(Guid id)
        {
            return await Get<Organisation>($"/organisations/id/{id}");
        }
        
        private async Task<T> Get<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", await _tokenService.GetToken());

            using (var response = await _client.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }
    }
}
