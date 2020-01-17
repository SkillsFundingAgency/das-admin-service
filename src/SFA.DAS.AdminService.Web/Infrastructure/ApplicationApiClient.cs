using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.Application.Api.Client.Clients;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UpdateFinancialsRequest = SFA.DAS.AssessorService.Api.Types.Models.Register.UpdateFinancialsRequest;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class ApplicationApiClient : ApiClientBase, IApplicationApiClient
    {
        //private readonly HttpClient _client;
        //private readonly ILogger<ApplicationApiClient> _logger;
        //private readonly ITokenService _tokenService;

        public ApplicationApiClient() : base()
        {

        }

        public ApplicationApiClient(HttpClient client, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(client, tokenService, logger)
        {
            //_client = client;
            //_logger = logger;
            //_tokenService = tokenService;
        }

        public ApplicationApiClient(string baseUri, ITokenService tokenService, ILogger<ApiClientBase> logger) : base(baseUri, tokenService, logger)
        {
            //_client = new HttpClient { BaseAddress = new Uri(baseUri) };
            //_logger = logger;
            //_tokenService = tokenService;
        }

        private async Task<T> Get<T>(string uri)
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

            using (var response = await HttpClient.GetAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        private async Task Post<T>(string uri, T model)
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await HttpClient.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json"))) { }
        }

        protected async Task<U> Put<T, U>(string uri, T model)
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await HttpClient.PutAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
            }
        }

        protected async Task<T> Delete<T>(string uri)
        {
            HttpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", TokenService.GetToken());

            using (var response = await HttpClient.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async virtual Task<ApplicationResponse> GetApplication(Guid Id)
        {
            return await Get<ApplicationResponse>($"/api/v1/Applications/{Id}/application");
        }

        #region Application
        public async virtual Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceNo)
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/OpenApplications?sequenceNo={sequenceNo}");
        }

        public async virtual Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/FeedbackAddedApplications");
        }

        public async virtual Task<List<ApplicationSummaryItem>> GetClosedApplications()
        {
            return await Get<List<ApplicationSummaryItem>>($"/Review/ClosedApplications");
        }

        public async virtual Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            await Post($"/Review/Applications/{applicationId}/Sequences/{sequenceNo}/Sections/{sectionNo}/StartReview", new { reviewer });
        }

        public async virtual Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceNo}/Sections/{sectionNo}/Evaluate",
                new { isSectionComplete, evaluatedBy });
        }

        public async virtual Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceNo}/Return", new { returnType, returnedBy });
        }
        #endregion

        #region Financial
        public async virtual Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/OpenApplications");
        }

        public async virtual Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/FeedbackAddedApplications");
        }

        public async virtual Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            return await Get<List<FinancialApplicationSummaryItem>>($"/Financial/ClosedApplications");
        }

        public async virtual Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            await Post($"/Financial/{applicationId}/StartReview", new { reviewer });
        }

        public async virtual Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade)
        {
            await Post($"/Financial/{applicationId}/Return", grade);
        }
        #endregion

        #region Feedback
        public async virtual Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/AddFeedback",
                feedback);
        }

        public async virtual Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            await Post(
                $"Review/Applications/{applicationId}/Sequences/{sequenceId}/Sections/{sectionId}/Pages/{pageId}/DeleteFeedback",
                feedbackId);
        }
        #endregion

        #region Answer Injection Service
        public async virtual Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            await Post("api/ao/assessment-organisations/update-financials", updateFinancialsRequest);
        }
        #endregion
    }
}
