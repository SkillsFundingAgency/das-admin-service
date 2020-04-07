using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AdminService.Web.Models;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;

namespace SFA.DAS.AdminService.Web.Infrastructure.RoatpClients
{
    public interface IRoatpApplicationApiClient
    {
        Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task<RoatpApplicationResponse> GetApplication(Guid Id);
        Task<List<RoatpApplicationSummaryItem>> GetClosedApplications();
        Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications();
        Task<List<RoatpApplicationSummaryItem>> GetFeedbackAddedApplications();
        Task<List<RoatpFinancialSummaryItem>> GetClarificationFinancialApplications();
        Task<List<RoatpApplicationSummaryItem>> GetOpenApplications();
        Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications();

        Task<RoatpFinancialApplicationsStatusCounts> GetFinancialApplicationsStatusCounts();

        Task ReturnApplication(Guid applicationId, string returnType, string returnedBy);
        Task ReturnFinancialReview(Guid applicationId, FinancialReviewDetails financialReviewDetails);
        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task StartFinancialReview(Guid applicationId, string reviewer);
        Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback);
        Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId);

        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);

        Task<List<RoatpSequence>> GetRoatpSequences();


        Task<List<RoatpApplicationSummaryItem>> GetNewGatewayApplications();
        Task<List<RoatpApplicationSummaryItem>> GetInProgressGatewayApplications();
        Task<List<RoatpApplicationSummaryItem>> GetClosedGatewayApplications();
        Task StartGatewayReview(Guid applicationId, string reviewer);
        Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy);

        Task StartAssessorReview(Guid applicationId, string reviewer);

        Task<Guid> SnapshotApplication(Guid Id, Guid NewApplicationId, List<RoatpApplySequence> sequences);

       
         Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId);

         //MFCMFC THIS NEEDS TO GO ONCE ALL TIDY UP IS DONE
         Task<GatewayPageAnswer> GetGatewayPageAnswer(Guid applicationId, string pageId);

         Task<GatewayCommonDetails> GetPageCommonDetails(Guid applicationId, string pageId, string userName);
         Task<ContactAddress> GetOrganisationAddress(Guid applicationId);
        Task<string> GetIcoNumber(Guid applicationId);
        Task<string> GetTypeOfOrganisation(Guid applicationId);
        Task TriggerGatewayDataGathering(Guid applicationId, string userName);

         Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string username,
            string comments);

        Task<ProviderDetails> GetUkrlpDetails(Guid applicationId);

        Task<CompaniesHouseSummary> GetCompaniesHouseDetails(Guid applicationId);

        Task<CharityCommissionSummary> GetCharityCommissionDetails(Guid applicationId);

        Task<OrganisationRegisterStatus> GetOrganisationRegisterStatus(Guid applicationId);

        Task<DateTime?> GetSourcesCheckedOnDate(Guid applicationId);
        Task<string> GetTradingName(Guid applicationId);
        Task<string> GetWebsiteAddressSourcedFromUkrlp(Guid applicationId);
        Task<string> GetWebsiteAddressManuallyEntered(Guid applicationId);
        Task<string> GetOrganisationWebsiteAddress(Guid applicationId);
        Task<string> GetOfficeForStudents(Guid applicationId);
        Task<string> GetInitialTeacherTraining(Guid applicationId);
    }
}
