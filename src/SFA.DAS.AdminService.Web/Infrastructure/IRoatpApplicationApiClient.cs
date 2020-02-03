using SFA.DAS.AssessorService.Api.Types.Models.Register;
using SFA.DAS.AssessorService.ApplyTypes;
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
        Task ReturnFinancialReview(Guid applicationId, FinancialGrade grade);
        Task StartApplicationSectionReview(Guid applicationId, int sequenceNo, int sectionNo, string reviewer);
        Task StartFinancialReview(Guid applicationId, string reviewer);
        Task AddFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Feedback feedback);
        Task DeleteFeedback(Guid applicationId, int sequenceId, int sectionId, string pageId, Guid feedbackId);

        Task UpdateFinancials(UpdateFinancialsRequest updateFinancialsRequest);

        Task<List<RoatpSequence>> GetRoatpSequences();


        Task<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>> GetNewGatewayApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>> GetInProgressGatewayApplications();
        Task<List<AssessorService.ApplyTypes.Roatp.RoatpApplicationSummaryItem>> GetClosedGatewayApplications();
        Task StartGatewayReview(Guid applicationId, string reviewer);
        Task EvaluateGateway(Guid applicationId, bool isGatewayApproved, string evaluatedBy);
    }

    public class RoatpApplicationResponse // Update this copy in Apply API too!
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public FinancialGrade financialGrade { get; set; } // Not sure why this is here. Old code perhaps?
        public string ApplicationStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public string FinancialReviewStatus { get; set; } // Not sure why this is here. Old code perhaps?
        public RoatpApplyData ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public int? StandardCode { get; set; }
    }

    public class RoatpApplyData
    {
        public List<RoatpApplySequence> Sequences { get; set; }
        public RoatpApplyDetails ApplyDetails { get; set; }
    }

    public class RoatpApplyDetails
    {
        public string ReferenceNumber { get; set; }
        public string UKPRN { get; set; }
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public int ProviderRoute { get; set; } 
        public DateTime? ApplicationSubmittedOn { get; set; }
        public Guid? ApplicationSubmittedBy { get; set; }
    }

    public class RoatpApplySequence
    {
        public Guid SequenceId { get; set; }
        public int SequenceNo { get; set; }
        public List<RoatpApplySection> Sections { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public bool NotRequired { get; set; }
        public bool Sequential { get; set; }
        public string Description { get; set; }
        //public DateTime? ApprovedDate { get; set; }
        //public string ApprovedBy { get; set; }
    }

    public class RoatpApplySection
    {
        public Guid SectionId { get; set; }
        public int SectionNo { get; set; }
        public string Status { get; set; }
        //public DateTime? ReviewStartDate { get; set; }
        //public string ReviewedBy { get; set; }
        //public DateTime? EvaluatedDate { get; set; }
        //public string EvaluatedBy { get; set; }
        public bool NotRequired { get; set; }
        //public bool? RequestedFeedbackAnswered { get; set; }
    }

    public class RoatpSequence
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool Sequential { get; set; }
        public List<string> ExcludeSections { get; set; }
        public List<string> Roles { get; set; }
    }
}
