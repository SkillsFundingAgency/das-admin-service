﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.AdminService.Infrastructure.ApiClients.RoatpApplication.Types.Apply
{
    //@ should be removed as part of snapshot code cleanup APR-2975
    public class RoatpApply
    {
        public Guid ApplicationId { get; set; }
        public Guid OrganisationId { get; set; }

        public string ApplicationStatus { get; set; }
        public string AssessorReviewStatus { get; set; }
        public string GatewayReviewStatus { get; set; }

        public RoatpApplyData ApplyData { get; set; }

        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }

        public string Comments { get; set; }
        public string ExternalComments { get; set; }
    }

    //@ should be removed as part of snapshot code cleanup APR-2975
    public class RoatpApplyData
    {
        public List<RoatpApplySequence> Sequences { get; set; }
        public RoatpApplyDetails ApplyDetails { get; set; }
        public RoatpApplyGatewayDetails GatewayReviewDetails { get; set; }
    }

    //@ should be removed as part of snapshot code cleanup APR-2975
    public class RoatpApplyGatewayDetails
    {
        public DateTime? SourcesCheckedOn { get; set; }
        public string Comments { get; set; }
        public DateTime? OutcomeDateTime { get; set; }
        public string GatewaySubcontractorDeclarationClarificationUpload { get; set; }
    }

    public class RoatpApplyDetails
    {
        public string ReferenceNumber { get; set; }
        public string UKPRN { get; set; }
        public string OrganisationName { get; set; }
        public string TradingName { get; set; }
        public int ProviderRoute { get; set; }
        public string ProviderRouteName { get; set; }
        public DateTime? ApplicationSubmittedOn { get; set; }
        public Guid? ApplicationSubmittedBy { get; set; }
        public DateTime? ApplicationWithdrawnOn { get; set; }
        public string ApplicationWithdrawnBy { get; set; }
        public DateTime? ApplicationRemovedOn { get; set; }
        public string ApplicationRemovedBy { get; set; }
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
