using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Application;
using SFA.DAS.RoatpAssessor.Application.Gateway.Commands;
using SFA.DAS.RoatpAssessor.Domain.DTOs;
using SFA.DAS.RoatpAssessor.Services.ApplyApiClient.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.RoatpAssessor.Services.ApplyApiClient
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

        public Task<List<Gateway>> GetInProgressAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/roatp-assessor/gateway/in-progress"))
            {
                return RequestAndDeserialiseAsync<List<Domain.DTOs.Gateway>>(request);
            }
        }

        public Task<GatewayCounts> GetGatewayCountsAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"roatp-assessor/gateway/counts"))
            {
                return RequestAndDeserialiseAsync<GatewayCounts>(request);
            }
        }

        public Task UpdateGatewayOutcomesAsync(UpdateGatewayOutcomesCommand command)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"roatp-assessor/gateway/outcomes"))
            {
                return PostPutRequest(request, command);
            }
        }

        public Task<Gateway> GetGatewayReviewAsync(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"roatp-assessor/gateway/{applicationId}"))
            {
                return RequestAndDeserialiseAsync<Gateway>(request);
            }
        }

        public Task<Domain.DTOs.Application> GetApplicationAsync(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Application/{applicationId}"))
            {
                return RequestAndDeserialiseAsync<Domain.DTOs.Application>(request);
            }
        }

        public Task CreateGatewayAsync(CreateGatewayModel model)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"roatp-assessor/gateway/create"))
            {
                return PostPutRequest(request, model);
            }
        }
    }
}