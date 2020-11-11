using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using SFA.DAS.AssessorService.ApplyTypes.Roatp.Apply;
using SFA.DAS.AssessorService.Domain.Entities;

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

        Task<Contact> GetContactForApplication(Guid applicationId);

        Task<List<RoatpSequence>> GetRoatpSequences();

        Task StartAssessorReview(Guid applicationId, string reviewer);

        Task<Guid> SnapshotApplication(Guid Id, Guid NewApplicationId, List<RoatpApplySequence> sequences);


        Task<bool> UploadClarificationFile(Guid applicationId, IFormFileCollection clarificationFiles);

    }
}
