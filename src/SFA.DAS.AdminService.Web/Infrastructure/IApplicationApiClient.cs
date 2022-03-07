using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.AssessorService.Api.Types.Models.Apply.Review;
using SFA.DAS.AssessorService.ApplyTypes;
using SFA.DAS.AssessorService.Domain.Entities;
using SFA.DAS.AssessorService.Domain.Paging;
using FinancialGrade = SFA.DAS.AssessorService.ApplyTypes.FinancialGrade;
using UpdateFinancialsRequest = SFA.DAS.AssessorService.Api.Types.Models.Register.UpdateFinancialsRequest;

namespace SFA.DAS.AdminService.Web.Infrastructure
{
    public interface IApplicationApiClient
    {
        Task EvaluateSection(Guid applicationId, int sequenceNo, int sectionNo, bool isSectionComplete, string evaluatedBy);
        Task<ApplicationResponse> GetApplication(Guid Id);
        Task<List<ApplicationResponse>> GetWithdrawnApplications(Guid orgId, int? StandardCode);
        Task<ApplicationReviewStatusCounts> GetApplicationReviewStatusCounts();
        Task<PaginatedList<ApplicationSummaryItem>> GetOrganisationApplications(OrganisationApplicationsRequest organisationApplicationsRequest);
        Task<PaginatedList<ApplicationSummaryItem>> GetWithdrawalApplications(WithdrawalApplicationsRequest organisationApplicationsRequest);
        Task<PaginatedList<ApplicationSummaryItem>> GetStandardApplications(StandardApplicationsRequest standardApplicationsRequest);

        Task ReturnApplicationSequence(Guid applicationId, int sequenceNo, string returnType, string returnedBy);

        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);
    }

    public class ApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationType { get; set; }
        public Guid OrganisationId { get; set; }
        public string EndPointAssessorName { get; set; }
        public FinancialGrade FinancialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }
        public int? StandardCode { get; set; }
        public string CreatedBy { get; set; }
        public string ContactName { get; set; }
        public string ContactEmail { get; set; }
        public string StandardApplicationType { get; set; }
    }
}