using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public class RoatpApplicationApiClient : IRoatpApplicationApiClient
    {
        public Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId)
        {
            throw new NotImplementedException();
        }

        public Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy)
        {
            throw new NotImplementedException();
        }

        public Task<ApplicationResponse> GetApplication(Guid Id)
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationSummaryItem>> GetClosedApplications()
        {
            throw new NotImplementedException();
        }

        public Task<List<FinancialApplicationSummaryItem>> GetClosedFinancialApplications()
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationSummaryItem>> GetFeedbackAddedApplications()
        {
            throw new NotImplementedException();
        }

        public Task<List<FinancialApplicationSummaryItem>> GetFeedbackAddedFinancialApplications()
        {
            throw new NotImplementedException();
        }

        public Task<List<ApplicationSummaryItem>> GetOpenApplications(int sequenceNo)
        {
            throw new NotImplementedException();
        }

        public Task<List<FinancialApplicationSummaryItem>> GetOpenFinancialApplications()
        {
            throw new NotImplementedException();
        }

        public Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy)
        {
            throw new NotImplementedException();
        }

        public Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade)
        {
            throw new NotImplementedException();
        }

        public Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer)
        {
            throw new NotImplementedException();
        }

        public Task StartFinancialReview(Guid applicationId, string reviewer)
        {
            throw new NotImplementedException();
        }

        public Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest)
        {
            throw new NotImplementedException();
        }
    }
}
