using System;
using System.Collections.Generic;

namespace SFA.DAS.AssessorService.ApplyTypes.Roatp
{
    public class Apply
    {
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string ApplicationStatus { get; set; }
        public string ReviewStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public ApplyData ApplyData { get; set; }

        public Guid Id { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public DateTime? FeedbackAddedDate { get; set; }
        public DateTime? SubmittedDate { get; set; }
        public DateTime? ClosedDate { get; set; }
    }

    public class ApplyData
    {
        public List<ApplySequence> Sequences { get; set; }
        public ApplyDetails ApplyDetails { get; set; }
        public FinancialReviewDetails FinancialReviewDetails { get; set; }
    }

    public class ApplyDetails
    {
        // NOTE THIS IS A SIMILAR COPY OF RoatpApplicationData
        public string ReferenceNumber { get; set; }
        public string UKPRN { get; set; }
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public string ProviderRoute { get; set; } // was string - ApplicationRouteId
        public DateTime? ApplicationSubmittedOn { get; set; }
        public Guid? ApplicationSubmittedBy { get; set; }
    }

    //public class ApplySequence
    //{
    //    public Guid SequenceId { get; set; }
    //    public int SequenceNo { get; set; }
    //    public List<ApplySection> Sections { get; set; }
    //    //public string Status { get; set; }
    //    //public bool IsActive { get; set; }
    //    public bool NotRequired { get; set; }
    //    //public bool Sequential { get; set; }
    //    //public string Description { get; set; }
    //    //public DateTime? ApprovedDate { get; set; }
    //    //public string ApprovedBy { get; set; }
    //}

    //public class ApplySection
    //{
    //    public Guid SectionId { get; set; }
    //    public int SectionNo { get; set; }
    //    public string Status { get; set; }
    //    //public DateTime? ReviewStartDate { get; set; }
    //    //public string ReviewedBy { get; set; }
    //    //public DateTime? EvaluatedDate { get; set; }
    //    //public string EvaluatedBy { get; set; }
    //    public bool NotRequired { get; set; }
    //    //public bool? RequestedFeedbackAnswered { get; set; }
    //}

    public class FinancialReviewDetails
    {
        public string SelectedGrade { get; set; }
        public DateTime? FinancialDueDate { get; set; }
        public string GradedBy { get; set; }
        public DateTime? GradedDateTime { get; set; }
        public string Comments { get; set; }
        public List<FinancialEvidence> FinancialEvidences { get; set; }
    }

    public class FinancialEvidence
    {
        public string Filename { get; set; }
    }

    public class RoatpApplicationResponse
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }
        public FinancialGrade financialGrade { get; set; }
        public string ApplicationStatus { get; set; }
        public string FinancialReviewStatus { get; set; }
        public string GatewayReviewStatus { get; set; }
        public RoatpApplyData ApplyData { get; set; }
        public string CreatedBy { get; set; }
        public int? StandardCode { get; set; }
    }

    public class RoatpApplyData
    {
        public List<RoatpApplySequence> Sequences { get; set; }
        public RoatpApplyDetails ApplyDetails { get; set; }
        public FinancialReviewDetails FinancialReviewDetails { get; set; }
    }

    public class RoatpApplyDetails
    {
        public string ReferenceNumber { get; set; }
        public string UKPRN { get; set; }
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public string ProviderRoute { get; set; }
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
