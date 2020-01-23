using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
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
            return await Task.FromResult(new Organisation());
        }

        public async Task<List<Contact>> GetOrganisationContacts(Guid organisationId)
        {
            return await Task.FromResult(new List<Contact>());
        }
    }
}
