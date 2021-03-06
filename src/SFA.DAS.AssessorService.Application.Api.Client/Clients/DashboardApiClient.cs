﻿using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Dashboard;
using System.Net.Http;
using System.Threading.Tasks;

namespace SFA.DAS.AssessorService.Application.Api.Client.Clients
{
    public class DashboardApiClient : ApiClientBase, IDashboardApiClient
    {
        private readonly ILogger<DashboardApiClient> _logger;

        public DashboardApiClient(string baseUri, ITokenService tokenService, ILogger<DashboardApiClient> logger) : base(baseUri, tokenService, logger)
        {
            _logger = logger;
        }

        public async Task<GetEpaoDashboardResponse> GetEpaoDashboard(string epaoId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/dashboard/{epaoId}"))
            {
                return await RequestAndDeserialiseAsync<GetEpaoDashboardResponse>(request, $"Could not get the epao dashboard for epao: {epaoId}");
            }
        }
    }
}