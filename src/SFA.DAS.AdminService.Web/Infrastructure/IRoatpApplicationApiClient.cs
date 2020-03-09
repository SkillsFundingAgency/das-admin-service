using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.UKRLP;
using SFA.DAS.AssessorService.ApplyTypes.CharityCommission;
using SFA.DAS.AssessorService.ApplyTypes.CompaniesHouse;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IRoatpApplicationApiClient
    {
        Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task<RoatpApplicationResponse> GetApplication(Guid Id);
        Task<List<RoatpApplicationSummaryItem>> GetClosedApplications();
        Task<List<RoatpFinancialSummaryItem>> GetClosedFinancialApplications();
        Task<List<RoatpApplicationSummaryItem>> GetFeedbackAddedApplications();
        Task<List<RoatpFinancialSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<RoatpApplicationSummaryItem>> GetOpenApplications();
        Task<List<RoatpFinancialSummaryItem>> GetOpenFinancialApplications();
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

        Task<CompaniesHouseSummary> GetCompanyDetails(string companiesHouseNumber);
        Task<Charity> GetCharityDetails(string charityNumber);
         Task<List<GatewayPageAnswerSummary>> GetGatewayPageAnswers(Guid applicationId);
         Task<GatewayPageAnswer> GetGatewayPageAnswer(Guid applicationId, string pageId);
         Task SubmitGatewayPageAnswer(Guid applicationId, string pageId, string status, string username,
            string gatewayPageData);
    }
}
