using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.ApplyTypes;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApplicationApiClient
    {
        Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task<ApplicationResponse> GetApplication(Guid Id);
        Task<List<ApplicationSummaryItem>> GetClosedApplications();
        Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications();
        Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications();
        Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications();
        Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceNo);
        Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications();
        Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy);
        Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade);
        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task StartFinancialReview(Guid applicationId, string reviewer);
        Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback);
        Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId);
    }
}