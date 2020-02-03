using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpApplicationApiClient : IRoatpApplicationApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<RoatpApplicationApiClient> _logger;
        private readonly IRoatpApplyTokenService _tokenService;

        public RoatpApplicationApiClient(string baseUri, ILogger<RoatpApplicationApiClient> logger, IRoatpApplyTokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
        }

        public async Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback)
        {
            
        }

        public async Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            
        }

        public async Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            
        }

        public async Task<RoatpApplicationResponse> GetApplication(Guid applicationId)
        {
            return await Get<RoatpApplicationResponse>($"/Application/{applicationId}");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetClosedApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.Apply>>($"/Applications/Closed");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetClosedFinancialApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.Apply>>($"/Financial/ClosedApplications");            
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetFeedbackAddedApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.Apply>>($"/Applications/FeedbackAdded");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetFeedbackAddedFinancialApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.Apply>>($"/Financial/FeedbackAddedApplications");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetOpenApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.Apply>>($"/Applications/Open");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetOpenFinancialApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.Apply>>($"/Financial/OpenApplications");
        }

        public async Task ReturnApplication(Guid applicationId, string returnType, string returnedBy)
        {
            
        }

        public async Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade)
        {
            
        }

        public async Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            
        }

        public async Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            
        }

        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            
        }

        public async Task<List<RoatpSequence>> GetRoatpSequences()
        {
            return await Get<List<RoatpSequence>>($"/roatp-sequences");
        }




        public async Task<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>> GetNewGatewayApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>>($"/GatewayReview/NewApplications");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>> GetInProgressGatewayApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>>($"/GatewayReview/InProgressApplications");
        }

        public async Task<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>> GetClosedGatewayApplications()
        {
            return await Get<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>>($"/GatewayReview/ClosedApplications");
        }

        public async Task StartGatewayReview(Guid applicationId, string reviewer)
        {
            await Post($"/GatewayReview/{applicationId}/StartReview", new { reviewer });
        }

        public async Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy)
        {
            await Post($"/GatewayReview/{applicationId}/Evaluate", new { isGatewayApproved, evaluatedBy });
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

        private async Task Post<T>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) { }
        }

        private async Task<U> Put<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PutAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

    }
}
