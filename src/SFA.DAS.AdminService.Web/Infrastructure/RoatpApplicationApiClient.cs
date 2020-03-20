using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using System.Net.Http.Formatting;

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

        public async Task EvaluateSection(Guid applicationId, int sequenceId, int sectionId, bool sectionCompleted, string reviewer)
        {
            await Post($"/Application/{applicationId}/AssessorEvaluateSection", new { applicationId, sequenceId, sectionId, sectionCompleted, reviewer });
        }

        public async Task<RoatpApplicationResponse> GetApplication(Guid applicationId)
        {
            return await Get<RoatpApplicationResponse>($"/Application/{applicationId}");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetClosedApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/Applications/Closed");
        }

        public async Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications()
        {
            return await Get<List<RoatpFinancialSummaryItem>>($"/Financial/ClosedApplications");            
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/Applications/FeedbackAdded");
        }

        public async Task<List<RoatpFinancialSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            return await Get<List<RoatpFinancialSummaryItem>>($"/Financial/FeedbackAddedApplications");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetOpenApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/Applications/Open");
        }

        public async Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications()
        {
            return await Get<List<RoatpFinancialSummaryItem>>($"/Financial/OpenApplications");
        }

        public async Task ReturnApplication(Guid applicationId, string returnType, string returnedBy)
        {
            
        }

        public async Task ReturnFinancialReview(Guid applicationId, FinancialReviewDetails financialReviewDetails)
        {
            await Post<FinancialReviewDetails>($"/Financial/{applicationId}/Grade", financialReviewDetails);
        }

        public async Task StartApplicationSectionReview(Guid applicationId, int sequenceId, int sectionId, string reviewer)
        {
            await Post($"/Application/{applicationId}/StartAssessorSectionReview", 
                new { applicationId, sequenceId, sectionId, reviewer });
        }

        public async Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            await Post($"/Financial/{applicationId}/StartReview", new { reviewer });            
        }

        public async Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            
        }

        public async Task<List<RoatpSequence>> GetRoatpSequences()
        {
            return await Get<List<RoatpSequence>>($"/roatp-sequences");
        }




        public async Task<List<RoatpApplicationSummaryItem>> GetNewGatewayApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/NewApplications");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetInProgressGatewayApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/InProgressApplications");
        }

        public async Task<List<RoatpApplicationSummaryItem>> GetClosedGatewayApplications()
        {
            return await Get<List<RoatpApplicationSummaryItem>>($"/GatewayReview/ClosedApplications");
        }

        public async Task StartGatewayReview(Guid applicationId, string reviewer)
        {
            await Post($"/GatewayReview/{applicationId}/StartReview", new { reviewer });
        }

        public async Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy)
        {
            await Post($"/GatewayReview/{applicationId}/Evaluate", new { isGatewayApproved, evaluatedBy });
        }

        public async Task StartAssessorReview(Guid applicationId, string reviewer)
        {
            await Post($"/Application/{applicationId}/StartAssessorReview", new { reviewer });
        }

        public async Task<Guid> SnapshotApplication(Guid applicationId, Guid snapshotApplicationId, List<RoatpApplySequence> sequences)
        {
            return await Post<SnapshotApplicationRequest, Guid>($"/Application/Snapshot", new SnapshotApplicationRequest { ApplicationId = applicationId, SnapshotApplicationId = snapshotApplicationId, Sequences = sequences });
        }
  

        public async Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId)
        {
            return await Get<List<GatewayPageAnswerSummary>>($"/Gateway/Pages?applicationId={applicationId}");
        }

        //MFCMFC THIS NEEDS TO GO WHEN ALL TIDY UP IS DONE
        public async Task<GatewayPageAnswer> GetGatewayPageAnswer(Guid applicationId, string pageId)
        {
            return await Get<GatewayPageAnswer>($"/Gateway/Page/{applicationId}/{pageId}");
        }

        public async Task<GatewayCommonDetails> GetPageCommonDetails(Guid applicationId, string pageId, string userName)
        {
            return await Get<GatewayCommonDetails>($"Gateway/Page/CommonDetails/{applicationId}/{pageId}/{userName}");
        }

        public async Task<string> GetQnaCompanyAddress(Guid applicationId)
        {
            var result = await Get<GetQnaCompanyAddressResult>($"Gateway/Page/QnaCompanyAddress/{applicationId}");
            return result.Address;
        }

        public async Task TriggerGatewayDataGathering(Guid applicationId, string userName)
        {
            await Get<object>($"Gateway/ApiChecks/{applicationId}/{userName}");
        }

        public  async Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string username,
            string comments)
        {
            _logger.LogInformation($"RoatpApplicationApiClient-SubmitGatewayPageAnswer - ApplicationId '{applicationId}' - PageId '{pageId}' - Status '{status}' - UserName '{username}' - Comments '{comments}'");

            try
            {
                await Post($"/Gateway/Page/Submit", new { applicationId, pageId, status, comments, username });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "RoatpApplicationApiClient - SubmitGatewayPageAnswer - Error: '" + ex.Message + "'");
            }
            
        }

        public async Task<ProviderDetails> GetUkrlpDetails(Guid applicationId)
        {
            return await Get<ProviderDetails>($"Gateway/UkrlpData/{applicationId}");
        }

        public async Task<CompaniesHouseSummary> GetCompaniesHouseDetails(Guid applicationId)
        {
            return await Get<CompaniesHouseSummary>($"Gateway/CompaniesHouseData/{applicationId}");
        }

        public async Task<CharityCommissionSummary> GetCharityCommissionDetails(Guid applicationId)
        {
            return await Get<CharityCommissionSummary>($"Gateway/CharityCommissionData/{applicationId}");
        }

        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(Guid applicationId)
        {
            return await Get<OrganisationRegisterStatus>($"Gateway/RoatpRegisterData/{applicationId}");
        }

        public async Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId)
        {
            return await Get<DateTime?>($"Gateway/SourcesCheckedOn/{applicationId}");
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

    public class GetQnaCompanyAddressResult
    {
        public string Address { get; set; }
    }
}
