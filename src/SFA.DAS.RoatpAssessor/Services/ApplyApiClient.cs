using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Application;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Services
{
    public class ApplyApiClient : ApiClientBase, IApplyApiClient
    {
        private readonly ILogger<ApplyApiClient> _logger;

        public ApplyApiClient(string baseUri, IApplyTokenService tokenService, ILogger<ApplyApiClient> logger)
            : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public Task<List<Domain.DTOs.Application>> GetSubmittedApplicationsAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/roatp-assessor/applications/submitted"))
            {
                return RequestAndDeserialiseAsync<List<Domain.DTOs.Application>>(request);
            }
        }

        public Task<GatewayCounts> GetGatewayCounts()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"roatp-assessor/gateway/counts"))
            {
                return RequestAndDeserialiseAsync<GatewayCounts>(request);
            }
        }
    }
}