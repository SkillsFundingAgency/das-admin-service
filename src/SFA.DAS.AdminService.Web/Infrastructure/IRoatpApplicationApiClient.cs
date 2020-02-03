using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.ApplyTypes.Roatp;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IRoatpApplicationApiClient
    {
        Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task<RoatpApplicationResponse> GetApplication(Guid Id);
        Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetClosedApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetClosedFinancialApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetFeedbackAddedApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetFeedbackAddedFinancialApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetOpenApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.Apply>> GetOpenFinancialApplications();
        Task ReturnApplication(Guid applicationId, string returnType, string returnedBy);
        Task ReturnFinancialReview(Guid applicationId, FinancialReviewDetails grade);
        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task StartFinancialReview(Guid applicationId, string reviewer);
        Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback);
        Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId);

        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);

        Task<List<RoatpSequence>> GetRoatpSequences();
    }
    
}
