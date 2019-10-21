using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.RoatpAssessor.Domain.Entities;
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

        public async Task<IEnumerable<Domain.Entities.Application>> GetSubmittedApplicationsAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Applications/Status/Submitted"))
            {
                return await RequestAndDeserialiseAsync<List<Domain.Entities.Application>>(request);
            }
        }

        public async Task<Domain.Entities.Application> GetApplicationAsync(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/Application/{applicationId}"))
            {
                return await RequestAndDeserialiseAsync<Domain.Entities.Application>(request);
            }
        }

        public async Task<Guid> CreateApplicationReview(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/CreateReview/{applicationId}"))
            {
                return await RequestAndDeserialiseAsync<Guid>(request);
            }
        }

        public async Task<Domain.Entities.ApplicationReview> GetApplicationReviewAsync(Guid applicationId)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/ApplicationReview/{applicationId}"))
            {
                return await RequestAndDeserialiseAsync<Domain.Entities.ApplicationReview>(request);
            }
        }

        public async Task UpdateApplicationReviewGatewayReviewAsync(Guid applicationId, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateGatewayReview"))
            {
                await PostPutRequest<object>(request, new { GatewayReviewStatus = status.ToString() });
            }
        }

        public async Task UpdateApplicationReviewPmoReviewAsync(Guid applicationId, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdatePmoReview"))
            {
                await PostPutRequest<object>(request, new { PmoReviewStatus = status.ToString() });
            }
        }

        public async Task UpdateApplicationReviewAssessorReviewAsync(Guid applicationId, AssessorReviewNo reviewNo, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAssessorReview"))
            {
                await PostPutRequest<object>(request, new { reviewNo, AssessorReviewStatus = status.ToString() });
            }
        }

        public async Task<IEnumerable<ApplicationReview>> GetActiveApplicationReviewsAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/ApplicationReviews/Active"))
            {
                return await RequestAndDeserialiseAsync<List<Domain.Entities.ApplicationReview>>(request);
            }
        }

        public async Task UpdateAssessorComments(Guid applicationId, AssessorReviewNo reviewNo, Guid sectionId, string pageId, string comment)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAssessorComments"))
            {
                await PostPutRequest<object>(request, new { reviewNo, sectionId, pageId, comment });
            }
        }

        public async Task UpdateApplicationReviewAssessorModerationAsync(Guid applicationId, ApplicationReviewStatus status)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/ApplicationReview/{applicationId}/UpdateAssessorModeration"))
            {
                await PostPutRequest<object>(request, new { AssessorModerationStatus = status.ToString() });
            }
        }
    }
}
