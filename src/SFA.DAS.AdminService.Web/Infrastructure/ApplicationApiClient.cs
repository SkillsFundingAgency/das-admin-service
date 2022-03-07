using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.Application.Api.Client;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Paging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using UpdateFinancialsRequest = SFA.DAS.AssessorService.Api.Types.Models.Register.UpdateFinancialsRequest;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class ApplicationApiClient : IApplicationApiClient
    {
        private readonly HttpClient _client;
        private readonly ILogger<ApplicationApiClient> _logger;
        private readonly ITokenService _tokenService;

        public ApplicationApiClient(HttpClient client, ILogger<ApplicationApiClient> logger, ITokenService tokenService)
        {
            _client = client;
            _logger = logger;
            _tokenService = tokenService;
        }

        public ApplicationApiClient(string baseUri, ILogger<ApplicationApiClient> logger, ITokenService tokenService)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            _logger = logger;
            _tokenService = tokenService;
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

        private async Task<U> Post<T, U>(string uri, T model)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());
            var serializeObject = JsonConvert.SerializeObject(model);

            using (var response = await _client.PostAsync(new Uri(uri, UriKind.Relative),
                new StringContent(serializeObject, System.Text.Encoding.UTF8, "application/json")))
            {
                return await response.Content.ReadAsAsync<U>();
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

        protected async Task<U> Put<T, U>(string uri, T model)
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

        protected async Task<T> Delete<T>(string uri)
        {
            _client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _tokenService.GetToken());

            using (var response = await _client.DeleteAsync(new Uri(uri, UriKind.Relative)))
            {
                return await response.Content.ReadAsAsync<T>();
            }
        }

        public async Task<ApplicationResponse> GetApplication(Guid Id)
        {
            return await Get<ApplicationResponse>($"/api/v1/Applications/{Id}/application");
        }

        public async Task<List<ApplicationResponse>> GetWithdrawnApplications(Guid orgId, int? standardCode)
        {
            return await Get<List<ApplicationResponse>>($"/api/v1/Applications/{orgId}/application/withdrawn/{standardCode}");
        }


        #region Application
        public async Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts()
        {
            return await Get<ApplicationReviewStatusCounts>($"/Review/ApplicationReviewStatusCounts");
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> GetOrganisationApplications(OrganisationApplicationsRequest organisationApplicationsRequest)
        {
            return await Post<OrganisationApplicationsRequest, PaginatedList<ApplicationSummaryItem>>(
                $"/Review/OrganisationApplications", organisationApplicationsRequest);
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> GetStandardApplications(StandardApplicationsRequest standardApplicationsRequest)
        {
            return await Post<StandardApplicationsRequest, PaginatedList<ApplicationSummaryItem>>(
                $"/Review/StandardApplications", standardApplicationsRequest);
        }

        public async Task<PaginatedList<ApplicationSummaryItem>> GetWithdrawalApplications(WithdrawalApplicationsRequest withdrawalApplicationsRequest)
        {
            return await Post<WithdrawalApplicationsRequest, PaginatedList<ApplicationSummaryItem>>(
                $"/Review/WithdrawalApplications", withdrawalApplicationsRequest);
        }

        public async Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            await Post($"/Review/Applications/{applicationId}/Sequences/{sequenceNo}/Sections/{sectionNo}/StartReview", new { reviewer });
        }

        public async Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceNo}/Sections/{sectionNo}/Evaluate",
                new { isSectionComplete, evaluatedBy });
        }

        public async Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy)
        {
            await Post($"Review/Applications/{applicationId}/Sequences/{sequenceNo}/Return", new { returnType, returnedBy });
        }
        #endregion

        #region Answer Injection Service
        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            await Post("api/ao/assessment-organisations/update-financials", updateFinancialsRequest);
        }
        #endregion
    }
}
