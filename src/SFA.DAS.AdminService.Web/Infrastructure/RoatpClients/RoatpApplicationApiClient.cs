using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using System.Net.Http.Formatting;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AdminService.Web.Infrastructure.RoatpClients.Exceptions;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public class RoatpApplicationApiClient : RoatpApiClientBase<RoatpApplicationApiClient>, IRoatpApplicationApiClient
    {
        public RoatpApplicationApiClient(string baseUri, ILogger<RoatpApplicationApiClient> logger, IRoatpApplyTokenService tokenService) : base(baseUri, logger, tokenService)
        {
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
            try
            {
                return await Get<GatewayCommonDetails>($"Gateway/Page/CommonDetails/{applicationId}/{pageId}/{userName}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Gateway common details", ex);
                throw new ExternalApiException("An error occurred when retrieving Gateway common details", ex);
            }
        }

        public async Task<ContactAddress> GetOrganisationAddress(Guid applicationId)
        {
            return await Get<ContactAddress>($"/Gateway/{applicationId}/OrganisationAddress");
        }

        public async Task<string> GetIcoNumber(Guid applicationId)
        {
            return await Get($"/Gateway/{applicationId}/IcoNumber");
        }

        public async Task<string> GetTypeOfOrganisation(Guid applicationId)
        {
            return await Get($"/organisation/TypeOfOrganisation/{applicationId}");
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
            try
            {
                return await Get<ProviderDetails>($"Gateway/UkrlpData/{applicationId}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving UKRLP details", ex);
                throw new ExternalApiException("An error occurred when retrieving UKRLP details", ex);
            }
        }

        public async Task<CompaniesHouseSummary> GetCompaniesHouseDetails(Guid applicationId)
        {
            try 
            { 
                return await Get<CompaniesHouseSummary>($"Gateway/CompaniesHouseData/{applicationId}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Companies House details", ex);
                throw new ExternalApiException("An error occurred when retrieving Companies House details", ex);
            }
        }

        public async Task<CharityCommissionSummary> GetCharityCommissionDetails(Guid applicationId)
        {
            try 
            { 
                return await Get<CharityCommissionSummary>($"Gateway/CharityCommissionData/{applicationId}");
            }
            catch (RoatpApiClientException ex)
            {
                _logger.LogError("An error occurred when retrieving Charity Commission details", ex);
                throw new ExternalApiException("An error occurred when retrieving Charity Commission details", ex);
            }
        }

        public async Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(Guid applicationId)
        {
            try
            {
                return await Get<OrganisationRegisterStatus>($"Gateway/RoatpRegisterData/{applicationId}");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occurred when retrieving RoATP details", ex);
                throw new ExternalApiException("An error occurred when retrieving RoATP details", ex);
            }
        }

        public async Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId)
        {
            return await Get<DateTime?>($"Gateway/SourcesCheckedOn/{applicationId}");
        }

        public async Task<string> GetTradingName(Guid applicationId)
        {
            return await Get($"/Gateway/{applicationId}/TradingName");
        }


        public async Task<string> GetWebsiteAddressSourcedFromUkrlp(Guid applicationId)
        {
            return await Get($"/Gateway/{applicationId}/WebsiteAddressFromUkrlp");
        }


        public async Task<string> GetWebsiteAddressManuallyEntered(Guid applicationId)
        {
            return await Get($"/Gateway/{applicationId}/WebsiteAddressManuallyEntered");
        }

        public async Task<string> GetOrganisationWebsiteAddress(Guid applicationId)
        {
            return await Get($"/Gateway/{applicationId}/OrganisationWebsiteAddress");
        }

        public async Task<string> GetOfficeForStudents(Guid applicationId)
        {
            return await Get($"/Accreditation/{applicationId}/OfficeForStudents");
        }

        public async Task<string> GetInitialTeacherTraining(Guid applicationId)
        {
            return await Get($"/Accreditation/{applicationId}/InitialTeacherTraining");
        }
    }
}
